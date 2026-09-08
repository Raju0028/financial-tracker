using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using FinancialTracker.Models;
using System.Text;

namespace FinancialTracker.Services;

public class LoanService
{
    private readonly string _spreadsheetId;
    private readonly TransactionService _transactionService;
    private readonly SheetsService _sheetsService;

    public LoanService(IConfiguration configuration, GoogleSheetsClientService googleSheetsClient, TransactionService transactionService)
    {
        _sheetsService = googleSheetsClient.SheetsService;
        _spreadsheetId = googleSheetsClient.SpreadsheetId;
        _transactionService = transactionService;
    }

    public async Task<List<Loan>> GetLoansAsync()
    {
        var range = "Loan!A:F";

        var request = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await request.ExecuteAsync();

        var rows = response.Values;

        var loans = new List<Loan>();

        if (rows == null || rows.Count <= 1)
        {
            return loans;
        }

        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];

            var loan = new Loan
            {
                RowNumber = i + 1,

                Date = row.Count > 0
            ? row[0]?.ToString() ?? string.Empty
            : string.Empty,

                LoanAmount = row.Count > 1 &&
                     decimal.TryParse(
                         row[1]?.ToString(),
                         out var loanAmount)
            ? loanAmount
            : 0,

                Duration = row.Count > 2
            ? row[2]?.ToString() ?? string.Empty
            : string.Empty,

                From = row.Count > 3
            ? row[3]?.ToString() ?? string.Empty
            : string.Empty,

                TotalLoan = row.Count > 4 &&
                    decimal.TryParse(
                        row[4]?.ToString(),
                        out var totalLoan)
            ? totalLoan
            : 0,

                Status = row.Count > 5
            ? row[5]?.ToString() ?? string.Empty
            : string.Empty
            };

            loans.Add(loan);
        }

        return loans;
    }

    public async Task<List<LoanRepayment>> GetLoanRepaymentsAsync()
    {
        var range = "Loan!H:J";

        var request = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await request.ExecuteAsync();

        var rows = response.Values;

        var repayments = new List<LoanRepayment>();

        if (rows == null || rows.Count <= 1)
        {
            return repayments;
        }

        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];

            var repayment = new LoanRepayment
            {
                RowNumber = i + 1,

                Date = row.Count > 0
                    ? row[0]?.ToString() ?? string.Empty
                    : string.Empty,

                RepaymentAmount = row.Count > 1 &&
                                  decimal.TryParse(
                                      row[1]?.ToString(),
                                      out var repaymentAmount)
                    ? repaymentAmount
                    : 0,

                To = row.Count > 2
                    ? row[2]?.ToString() ?? string.Empty
                    : string.Empty
            };

            repayments.Add(repayment);
        }

        return repayments;
    }

    public async Task<LoanSummary> GetLoanSummaryAsync()
    {
        var loans = await GetLoansAsync();
        var repayments = await GetLoanRepaymentsAsync();

        var totalLoan = loans.Sum(x => x.TotalLoan);

        var totalPaid = repayments.Sum(x => x.RepaymentAmount);

        var balance = totalLoan - totalPaid;

        if (balance < 0)
        {
            balance = 0;
        }

        var paidPercentage = totalLoan > 0
            ? (totalPaid / totalLoan) * 100
            : 0;

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var currentMonthRepayments = repayments
     .Where(x =>
         DateTime.TryParseExact(
             x.Date,
              "MM/dd/yyyy",
             System.Globalization.CultureInfo.InvariantCulture,
             System.Globalization.DateTimeStyles.None,
             out var date) &&
         date.Month == currentMonth &&
         date.Year == currentYear)
     .ToList();

        var paidThisMonth = currentMonthRepayments.Sum(
            x => x.RepaymentAmount);

        return new LoanSummary
        {
            TotalLoan = totalLoan,
            TotalPaid = totalPaid,
            Balance = balance,
            PaidPercentage = paidPercentage,
            PaidThisMonth = paidThisMonth
        };
    }

    public async Task AddLoanAsync(Loan loan)
    {
        // Get existing loan rows to find the next available row.
        var getRequest = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            "Loan!A:F");

        var response = await getRequest.ExecuteAsync();

        var rows = response.Values ?? new List<IList<object>>();

        var nextRow = 2;

        for (var i = 1; i < rows.Count; i++)
        {
            if (rows[i].Count > 0 &&
                !string.IsNullOrWhiteSpace(rows[i][0]?.ToString()))
            {
                nextRow = i + 2;
            }
        }
        Console.WriteLine($"Loan will be written to row: {nextRow}");
        // Write only A:D.
        // Column E contains the Google Sheet formula, so we don't overwrite it.
        var loanValues = new List<IList<object>>
    {
        new List<object>
        {
            loan.Date,
            loan.LoanAmount,
            loan.Duration,
            loan.From
        }
    };

        var loanValueRange =
            new Google.Apis.Sheets.v4.Data.ValueRange
            {
                Values = loanValues
            };

        var loanUpdateRequest =
            _sheetsService.Spreadsheets.Values.Update(
                loanValueRange,
                _spreadsheetId,
                $"Loan!A{nextRow}:D{nextRow}");

        loanUpdateRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

        await loanUpdateRequest.ExecuteAsync();

        // Write Status separately to column F.
        var statusValues = new List<IList<object>>
    {
        new List<object>
        {
            loan.Status
        }
    };

        var statusValueRange =
            new Google.Apis.Sheets.v4.Data.ValueRange
            {
                Values = statusValues
            };

        var statusUpdateRequest =
            _sheetsService.Spreadsheets.Values.Update(
                statusValueRange,
                _spreadsheetId,
                $"Loan!F{nextRow}:F{nextRow}");

        statusUpdateRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

        await statusUpdateRequest.ExecuteAsync();

        // Add to RecentTransactions only after successful Loan insertion.
        var transaction = new Transaction
        {
            Date = loan.Date,
            Description = $"Loan from {loan.From}",
            Category = "Loan",
            Type = "Income",
            Amount = loan.LoanAmount
        };

        await _transactionService.AddRecentTransactionAsync(transaction);
    }

    public async Task AddLoanRepaymentAsync(LoanRepayment repayment)
    {
        // Get existing repayment rows to find the next available row.
        var getRequest = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            "Loan!H:J");

        var response = await getRequest.ExecuteAsync();

        var rows = response.Values ?? new List<IList<object>>();

        var nextRow = 2;

        for (var i = 1; i < rows.Count; i++)
        {
            if (rows[i].Count > 0 &&
                !string.IsNullOrWhiteSpace(rows[i][0]?.ToString()))
            {
                nextRow = i + 2;
            }
        }

        var repaymentValues = new List<IList<object>>
    {
        new List<object>
        {
            repayment.Date,
            repayment.RepaymentAmount,
            repayment.To
        }
    };

        var valueRange =
            new Google.Apis.Sheets.v4.Data.ValueRange
            {
                Values = repaymentValues
            };

        var updateRequest =
            _sheetsService.Spreadsheets.Values.Update(
                valueRange,
                _spreadsheetId,
                $"Loan!H{nextRow}:J{nextRow}");

        updateRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

        await updateRequest.ExecuteAsync();

        // Add to RecentTransactions only after successful repayment insertion.
        var transaction = new Transaction
        {
            Date = repayment.Date,
            Description = $"Loan repayment to {repayment.To}",
            Category = "Loan Repayment",
            Type = "Expense",
            Amount = repayment.RepaymentAmount
        };

        await _transactionService.AddRecentTransactionAsync(
            transaction);
    }
}