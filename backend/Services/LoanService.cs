using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using FinancialTracker.Models;
using System.Text;

namespace FinancialTracker.Services;

public class LoanService
{
    private const string SpreadsheetId =
        "1aqI-obbI5llj3atRY_TdZZIr8VLisKFEBpMSeeX_HYY";

    private readonly SheetsService _sheetsService;

    public LoanService(IConfiguration configuration)
    {
        var credentialsBase64 =
            configuration["GoogleSheets:CredentialsBase64"]
            ?? throw new InvalidOperationException(
                "Google Sheets credentials are not configured.");

        var credentialsJson = Encoding.UTF8.GetString(
            Convert.FromBase64String(credentialsBase64));

        GoogleCredential credential =
            GoogleCredential.FromJson(credentialsJson)
            .CreateScoped(SheetsService.Scope.Spreadsheets);

        _sheetsService = new SheetsService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Financial Tracker"
            });
    }

    public async Task<List<Loan>> GetLoansAsync()
    {
        var range = "Loan!A:F";

        var request = _sheetsService.Spreadsheets.Values.Get(
            SpreadsheetId,
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
            SpreadsheetId,
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
        var values = new List<object>
    {
        loan.Date,
        loan.LoanAmount,
        loan.Duration,
        loan.From,
        loan.TotalLoan,
        loan.Status
    };

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
        {
            Values = new List<IList<object>>
        {
            values
        }
        };

        var request = _sheetsService.Spreadsheets.Values.Append(
            valueRange,
            SpreadsheetId,
            "Loan!A:F");

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest
                .ValueInputOptionEnum.USERENTERED;

        request.InsertDataOption =
            SpreadsheetsResource.ValuesResource.AppendRequest
                .InsertDataOptionEnum.INSERTROWS;

        await request.ExecuteAsync();
    }

    public async Task AddLoanRepaymentAsync(LoanRepayment repayment)
    {
        var values = new List<object>
    {
        repayment.Date,
        repayment.RepaymentAmount,
        repayment.To
    };

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
        {
            Values = new List<IList<object>>
        {
            values
        }
        };

        var request = _sheetsService.Spreadsheets.Values.Append(
            valueRange,
            SpreadsheetId,
            "Loan!H:J");

        request.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest
                .ValueInputOptionEnum.USERENTERED;

        request.InsertDataOption =
            SpreadsheetsResource.ValuesResource.AppendRequest
                .InsertDataOptionEnum.INSERTROWS;

        await request.ExecuteAsync();
    }
}