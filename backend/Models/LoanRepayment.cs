namespace FinancialTracker.Models;

public class LoanRepayment
{
    public int RowNumber { get; set; }

    public string Date { get; set; } = string.Empty;

    public decimal RepaymentAmount { get; set; }

    public string To { get; set; } = string.Empty;
}