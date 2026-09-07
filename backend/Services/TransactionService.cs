using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using FinancialTracker.Models;
using System.Globalization;
using System.Text;

namespace FinancialTracker.Services;

public class TransactionService
{
    private readonly string _spreadsheetId;

    private readonly SheetsService _sheetsService;

    public TransactionService(IConfiguration configuration, GoogleSheetsClientService googleSheetsClient)
    {
        _sheetsService = googleSheetsClient.SheetsService;
        _spreadsheetId = googleSheetsClient.SpreadsheetId;
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync()
    {
        const string range = "RecentTransactions!A2:E11";

        var request = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await request.ExecuteAsync();

        var rows = response.Values;

        var transactions = new List<Transaction>();

        if (rows == null)
        {
            return transactions;
        }

        foreach (var row in rows)
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

    public async Task AddRecentTransactionAsync(Transaction transaction)
    {
        const string range = "RecentTransactions!A2:E11";

        // Get existing recent transactions
        var getRequest = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await getRequest.ExecuteAsync();

        var recentTransactions = response.Values?
            .Select(row => new List<object>(row))
            .ToList()
            ?? new List<List<object>>();

        // Add the new transaction at the top
        recentTransactions.Insert(
            0,
            new List<object>
            {
            transaction.Date,
            transaction.Description,
            transaction.Category,
            transaction.Type,
            transaction.Amount
            });

        // Keep only the latest 10
        recentTransactions = recentTransactions
            .Take(10)
            .ToList();

        // Clear the existing data
        var clearRequest =
            _sheetsService.Spreadsheets.Values.Clear(
                new Google.Apis.Sheets.v4.Data.ClearValuesRequest(),
                _spreadsheetId,
                range);

        await clearRequest.ExecuteAsync();

        // Write the latest transactions
        if (recentTransactions.Count == 0)
        {
            return;
        }

        var valueRange =
            new Google.Apis.Sheets.v4.Data.ValueRange
            {
                Values = recentTransactions
                    .Select(row => (IList<object>)row)
                    .ToList()
            };

        var updateRequest =
            _sheetsService.Spreadsheets.Values.Update(
                valueRange,
                _spreadsheetId,
                range);

        updateRequest.ValueInputOption =
            Google.Apis.Sheets.v4.SpreadsheetsResource
                .ValuesResource
                .UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

        await updateRequest.ExecuteAsync();
    }

}