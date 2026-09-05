using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using FinancialTracker.Models;
using System.Globalization;

namespace FinancialTracker.Services;

public class GoogleSheetsService
{
	private const string SpreadsheetId =
	   "1aqI-obbI5llj3atRY_TdZZIr8VLisKFEBpMSeeX_HYY";

	private readonly SheetsService _sheetsService;

    public GoogleSheetsService(IConfiguration configuration)
    {
        var credentialsJson =
            configuration["GoogleSheets:CredentialsJson"]
            ?? throw new InvalidOperationException(
                "Google Sheets credentials are not configured.");

        GoogleCredential credential =
            GoogleCredential.FromJson(credentialsJson)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

        _sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Financial Tracker"
        });
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        var range = "Transactions!A:E";

        var request = _sheetsService.Spreadsheets.Values.Get(
            SpreadsheetId,
            range);

        var response = await request.ExecuteAsync();

        var rows = response.Values;

        var transactions = new List<Transaction>();

        if (rows == null || rows.Count <= 1)
        {
            return transactions;
        }

        foreach (var row in rows.Skip(1))
        {
            if (row.Count < 5)
            {
                continue;
            }

            var transaction = new Transaction
            {
                Date = row[0]?.ToString() ?? string.Empty,
                Description = row[1]?.ToString() ?? string.Empty,
                Category = row[2]?.ToString() ?? string.Empty,
                Type = row[3]?.ToString() ?? string.Empty,
                Amount = decimal.TryParse(
                    row[4]?.ToString(),
                    out var amount)
                    ? amount
                    : 0
            };

            transactions.Add(transaction);
        }

        return transactions;
    }

public async Task<List<OwnerList>> GetOwnerListsAsync()
{
    var range = "OwnedList!A:F";

    var request = _sheetsService.Spreadsheets.Values.Get(
        SpreadsheetId,
        range);

    var response = await request.ExecuteAsync();

    var rows = response.Values;

    var ownerLists = new List<OwnerList>();

    if (rows == null || rows.Count <= 1)
    {
        return ownerLists;
    }

        foreach (var row in rows.Skip(1))
        {
            var rowNumber = rows.IndexOf(row) + 1;

            var ownerList = new OwnerList
            {
                RowNumber = rowNumber,

                Date = row.Count > 0
                    ? row[0]?.ToString() ?? string.Empty
                    : string.Empty,

                Item = row.Count > 1
                    ? row[1]?.ToString() ?? string.Empty
                    : string.Empty,

                Cost = row.Count > 2 &&
                       decimal.TryParse(
                           row[2]?.ToString(),
                           NumberStyles.Any,
                           CultureInfo.InvariantCulture,
                           out var cost)
                    ? cost
                    : 0,

                Amount = row.Count > 3
                    ? row[3]?.ToString() ?? string.Empty
                    : string.Empty,

                Status = row.Count > 4
                    ? row[4]?.ToString() ?? string.Empty
                    : string.Empty,

                Comments = row.Count > 5
                    ? row[5]?.ToString() ?? string.Empty
                    : string.Empty
            };

            ownerLists.Add(ownerList);
        }
        // Latest date first
        ownerLists = ownerLists
        .OrderByDescending(x =>
            DateTime.TryParse(
                x.Date,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date)
                ? date
                : DateTime.MinValue)
        .ToList();

    return ownerLists;
}

    public async Task AddOwnerListAsync(OwnerList ownerList)
    {
        var range = "OwnedList!A:F";

        var values = new List<IList<object>>
    {
        new List<object>
        {
            ownerList.Date,
            ownerList.Item,
            ownerList.Cost,
            ownerList.Amount,
            ownerList.Status,
            ownerList.Comments
        }
    };

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
        {
            Values = values
        };

        var request = _sheetsService.Spreadsheets.Values.Append(
            valueRange,
            SpreadsheetId,
            range);

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        await request.ExecuteAsync();
    }

    public async Task UpdateOwnerListAsync(
    int rowNumber,
    OwnerList ownerList)
    {
        var range = $"OwnedList!A{rowNumber}:F{rowNumber}";

        var values = new List<IList<object>>
    {
        new List<object>
        {
            ownerList.Date,
            ownerList.Item,
            ownerList.Cost,
            ownerList.Amount,
            ownerList.Status,
            ownerList.Comments
        }
    };

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
        {
            Values = values
        };

        var request = _sheetsService.Spreadsheets.Values.Update(
            valueRange,
            SpreadsheetId,
            range);

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        await request.ExecuteAsync();
    }

    public async Task DeleteOwnerListAsync(int rowNumber)
    {
        var request = new Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Google.Apis.Sheets.v4.Data.Request>
        {
            new Google.Apis.Sheets.v4.Data.Request
            {
                DeleteDimension = new Google.Apis.Sheets.v4.Data.DeleteDimensionRequest
                {
                    Range = new Google.Apis.Sheets.v4.Data.DimensionRange
                    {
                        SheetId = await GetOwnedListSheetIdAsync(),
                        Dimension = "ROWS",
                        StartIndex = rowNumber - 1,
                        EndIndex = rowNumber
                    }
                }
            }
        }
        };

        var batchRequest =
            _sheetsService.Spreadsheets.BatchUpdate(
                request,
                SpreadsheetId);

        await batchRequest.ExecuteAsync();
    }

    private async Task<int> GetOwnedListSheetIdAsync()
    {
        var spreadsheetRequest =
            _sheetsService.Spreadsheets.Get(SpreadsheetId);

        var spreadsheet =
            await spreadsheetRequest.ExecuteAsync();

        var sheet = spreadsheet.Sheets
            .FirstOrDefault(x =>
                x.Properties?.Title == "OwnedList");

        if (sheet?.Properties?.SheetId == null)
        {
            throw new InvalidOperationException(
                "OwnedList sheet was not found.");
        }

        return sheet.Properties.SheetId.Value;
    }
}