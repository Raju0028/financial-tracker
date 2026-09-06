namespace FinancialTracker.Models;

public class LoanSummary
{
    public decimal TotalLoan { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal Balance { get; set; }

    public decimal PaidPercentage { get; set; }

    public decimal? PaidThisMonth { get; set; }
}