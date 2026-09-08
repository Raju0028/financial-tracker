using Google.Apis.Sheets.v4;
using FinancialTracker.Models;

namespace FinancialTracker.Services;

public class MoneyBorrowService
{
    private readonly string _spreadsheetId;
    private readonly SheetsService _sheetsService;
    private readonly MoneyBorrowSettings _settings;
    private readonly TransactionService _transactionService;


    public MoneyBorrowService(
        IConfiguration configuration,
        GoogleSheetsClientService googleSheetsClient,
        TransactionService transactionService)
    {
        _sheetsService = googleSheetsClient.SheetsService;
        _spreadsheetId = googleSheetsClient.SpreadsheetId;
        _transactionService = transactionService;

        _settings = configuration
            .GetSection("MoneyBorrow")
            .Get<MoneyBorrowSettings>()
            ?? throw new InvalidOperationException(
                "MoneyBorrow settings are not configured.");
    }

    public async Task<List<MoneyBorrow>> GetMoneyBorrowAsync()
    {
        var allBorrows = new List<MoneyBorrow>();

        foreach (var sheet in _settings.Sheets)
        {
            var range = $"'{sheet.Value}'!A2:E";

            var request = _sheetsService.Spreadsheets.Values.Get(
                _spreadsheetId,
                range);

            var response = await request.ExecuteAsync();

            if (response.Values == null)
            {
                continue;
            }

            for (var i = 0; i < response.Values.Count; i++)
            {
                var row = response.Values[i];

                if (row.Count < 3)
                {
                    continue;
                }

                allBorrows.Add(new MoneyBorrow
                {
                    RowNumber = i + 2,
                    SheetName = sheet.Value,
                    Date = FormatMoneyBorrowDate(
                        row[0]?.ToString()),

                    Amount = decimal.TryParse(
                        row[1]?.ToString(),
                        out var amount)
                        ? amount
                        : 0,

                    Bank = sheet.Key,

                    Person = row[2]?.ToString()
                        ?? string.Empty,

                    Comments = row.Count > 3
                        ? row[3]?.ToString()
                            ?? string.Empty
                        : string.Empty
                });
            }
        }

        return allBorrows
            .OrderByDescending(x => x.Date)
            .ToList();
    }

    public async Task AddMoneyBorrowAsync(MoneyBorrow moneyBorrow)
    {
        if (!_settings.Sheets.TryGetValue(moneyBorrow.Bank, out var sheetName))
        {
            throw new InvalidOperationException(
                $"No sheet is configured for bank: {moneyBorrow.Bank}");
        }

        var values = new List<IList<object>>
    {
        new List<object>
        {
            moneyBorrow.Date,
            moneyBorrow.Amount,
            moneyBorrow.Person,
            moneyBorrow.Comments
        }
    };

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
        {
            Values = values
        };

        var request = _sheetsService.Spreadsheets.Values.Append(
            valueRange,
            _spreadsheetId,
            $"'{sheetName}'!A:D");

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest
                .ValueInputOptionEnum.USERENTERED;

        request.InsertDataOption =
            SpreadsheetsResource.ValuesResource.AppendRequest
                .InsertDataOptionEnum.INSERTROWS;

        await request.ExecuteAsync();

        // Only after successful repayment insertion,
        // add it to RecentTransactions
        var transaction = new Transaction
        {
            Date = moneyBorrow.Date,
            Description = $"Money given to {moneyBorrow.Person}",
            Category = "Money Borrowed",
            Type = "Expense",
            Amount = moneyBorrow.Amount
        };

        await _transactionService.AddRecentTransactionAsync(
            transaction);
    }

    private static string FormatMoneyBorrowDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        if (double.TryParse(value, out var serialDate))
        {
            var date = DateTime.FromOADate(serialDate);

            return date.ToString("yyyy-MM-dd");
        }

        if (DateTime.TryParse(value, out var parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-dd");
        }

        return value;
    }

    public async Task DeleteMoneyBorrowAsync(
    string sheetName,
    int rowNumber)
    {
        var spreadsheet = await _sheetsService.Spreadsheets.Get(_spreadsheetId)
            .ExecuteAsync();

        var sheet = spreadsheet.Sheets
            .FirstOrDefault(x => x.Properties?.Title == sheetName);

        if (sheet?.Properties?.SheetId == null)
        {
            throw new InvalidOperationException(
                $"Sheet '{sheetName}' was not found.");
        }

        var requests = new List<Google.Apis.Sheets.v4.Data.Request>
    {
        new Google.Apis.Sheets.v4.Data.Request
        {
            DeleteDimension = new Google.Apis.Sheets.v4.Data.DeleteDimensionRequest
            {
                Range = new Google.Apis.Sheets.v4.Data.DimensionRange
                {
                    SheetId = sheet.Properties.SheetId.Value,
                    Dimension = "ROWS",
                    StartIndex = rowNumber - 1,
                    EndIndex = rowNumber
                }
            }
        }
    };

        var batchUpdateRequest =
            new Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest
            {
                Requests = requests
            };

        await _sheetsService.Spreadsheets.BatchUpdate(
            batchUpdateRequest,
            _spreadsheetId)
            .ExecuteAsync();
    }
}