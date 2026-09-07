using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using FinancialTracker.Models;
using System.Globalization;
using System.Text;

namespace FinancialTracker.Services;

public class OwnedListService
{
    private readonly string _spreadsheetId;
    private readonly TransactionService _transactionService;
    private readonly SheetsService _sheetsService;

    public OwnedListService(IConfiguration configuration, GoogleSheetsClientService googleSheetsClient, TransactionService transactionService)
    {
        _sheetsService = googleSheetsClient.SheetsService;
        _spreadsheetId = googleSheetsClient.SpreadsheetId;
        _transactionService = transactionService;
    }

    public async Task<List<OwnerList>> GetOwnerListsAsync()
    {
        var range = "OwnedList!A:F";

        var request = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
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
            _spreadsheetId,
            range);

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        await request.ExecuteAsync();

        // Only after successful insertion,
        // add it to RecentTransactions
        var transaction = new Transaction
        {
            Date = ownerList.Date,
            Description = ownerList.Item,
            Category = "Owned List",
            Type = "Expense",
            Amount = ownerList.Cost
        };

        await _transactionService.AddRecentTransactionAsync(
            transaction);
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
            _spreadsheetId,
            range);

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        // First add to OwnedList
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
                _spreadsheetId);

        await batchRequest.ExecuteAsync();
    }

    private async Task<int> GetOwnedListSheetIdAsync()
    {
        var spreadsheetRequest =
            _sheetsService.Spreadsheets.Get(_spreadsheetId);

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