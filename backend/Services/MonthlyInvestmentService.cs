using Google.Apis.Sheets.v4;
using FinancialTracker.Models;

namespace FinancialTracker.Services;

public class MonthlyInvestmentService
{
    private readonly string _spreadsheetId;
    private readonly TransactionService _transactionService;
    private readonly SheetsService _sheetsService;
    private readonly MonthlyPaymentSettings _settings;

    public MonthlyInvestmentService(IConfiguration configuration, GoogleSheetsClientService googleSheetsClient, TransactionService transactionService)
    {
        _sheetsService = googleSheetsClient.SheetsService;
        _spreadsheetId = googleSheetsClient.SpreadsheetId;
        _transactionService = transactionService;

        _settings = configuration
       .GetSection("MonthlyPayment")
       .Get<MonthlyPaymentSettings>()
       ?? throw new InvalidOperationException(
           "MonthlyPayment settings are not configured.");

    }

    public async Task<MonthlyInvestment> GetMonthlyInvestmentAsync()
    {
        var range = "MonthlyPayment!A4:AL4";

        var request = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await request.ExecuteAsync();

        var row = response.Values?.FirstOrDefault();

        if (row == null)
        {
            return new MonthlyInvestment();
        }

        return new MonthlyInvestment
        {
            // A4
            TotalAmount = GetDecimal(row, 0),

            // B4
            GasUsed = GetDecimal(row, 1),

            // D4
            Electricity = GetDecimal(row, 3),

            // F4
            Mobile = GetDecimal(row, 5),

            // J4
            Gift = GetDecimal(row, 9),

            // L4
            AuBank = GetDecimal(row, 11),

            // N4
            CarService = GetDecimal(row, 13),

            // P4
            CarInsurance = GetDecimal(row, 15),

            // R4
            Bike = GetDecimal(row, 17),

            // T4
            Cng = GetDecimal(row, 19),

            // V4
            Wifi = GetDecimal(row, 21),

            // Z4
            CcBank = GetDecimal(row, 25),

            // AB4
            Tour = GetDecimal(row, 27),

            // AD4
            FriendsTour = GetDecimal(row, 29),

            // AF4
            HouseProduct = GetDecimal(row, 31),

            // AH4
            EmergencyAmount = GetDecimal(row, 33),

            // AL4
            IdfcBank = GetDecimal(row, 37)
        };
    }

    private static decimal GetDecimal(
        IList<object> row,
        int index)
    {
        if (row.Count <= index)
        {
            return 0;
        }

        return decimal.TryParse(
            row[index]?.ToString(),
            out var value)
            ? value
            : 0;
    }

    public async Task AddMonthlyInvestmentAsync(
    AddMonthlyInvestment request)
    {
        if (string.IsNullOrWhiteSpace(request.Month))
        {
            throw new ArgumentException("Month is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Expense))
        {
            throw new ArgumentException("Expense is required.");
        }

        if (request.Prices == null || request.Prices.Count == 0)
        {
            throw new ArgumentException("At least one price is required.");
        }

        if (!_settings.Months.TryGetValue(
            request.Month,
            out var row))
        {
            throw new ArgumentException(
                $"Invalid month: {request.Month}");
        }

        if (!_settings.Expenses.TryGetValue(
            request.Expense,
            out var column))
        {
            throw new ArgumentException(
                $"Invalid expense: {request.Expense}");
        }

        var cell = $"{column}{row}";
        var range = $"'{_settings.SheetName}'!{cell}";

        var getRequest = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await getRequest.ExecuteAsync();

        var existingValue = response.Values?
            .FirstOrDefault()?
            .FirstOrDefault()?
            .ToString();

        var newValues = request.Prices
      .Select(price => price.ToString(
          System.Globalization.CultureInfo.InvariantCulture))
      .ToList();

        var formula = "=" + string.Join("+", newValues);

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
        {
            Values = new List<IList<object>>
        {
            new List<object> { formula }
        }
        };

        var updateRequest =
            _sheetsService.Spreadsheets.Values.Update(
                valueRange,
                _spreadsheetId,
                range);

        updateRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

        // First update MonthlyPayment
        await updateRequest.ExecuteAsync();

        // Calculate only the amount added in this operation
        var transactionAmount = request.Prices.Sum();

        // Only after successful insertion,
        // add it to RecentTransactions
        var transaction = new Transaction
        {
            Date = DateTime.Now.ToString("MM/dd/yyyy"),
            Description = request.Expense,
            Category = "Monthly Investment",
            Type = "Expense",
            Amount = transactionAmount
        };

        await _transactionService.AddRecentTransactionAsync(
            transaction);
    }

    public async Task<List<decimal>> GetExistingPricesAsync(
    string month,
    string expense)
    {
        if (string.IsNullOrWhiteSpace(month))
        {
            throw new ArgumentException("Month is required.");
        }

        if (string.IsNullOrWhiteSpace(expense))
        {
            throw new ArgumentException("Expense is required.");
        }

        if (!_settings.Months.TryGetValue(
            month,
            out var row))
        {
            throw new ArgumentException(
                $"Invalid month: {month}");
        }

        if (!_settings.Expenses.TryGetValue(
            expense,
            out var column))
        {
            throw new ArgumentException(
                $"Invalid expense: {expense}");
        }

        var cell = $"{column}{row}";
        var range = $"'{_settings.SheetName}'!{cell}";

        var getRequest = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await getRequest.ExecuteAsync();

        var existingValue = response.Values?
            .FirstOrDefault()?
            .FirstOrDefault()?
            .ToString();

        if (string.IsNullOrWhiteSpace(existingValue))
        {
            return new List<decimal>();
        }

        var formula = existingValue.Trim();

        if (formula.StartsWith("="))
        {
            formula = formula[1..];
        }

        var prices = formula
            .Split('+', StringSplitOptions.RemoveEmptyEntries)
            .Select(value =>
                decimal.TryParse(
                    value.Trim(),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var price)
                    ? (decimal?)price
                    : null)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .ToList();

        return prices;
    }
}