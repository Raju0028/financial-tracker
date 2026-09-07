namespace FinancialTracker.Models;

public class AddMonthlyInvestment
{
    public string Month { get; set; } = string.Empty;

    public string Expense { get; set; } = string.Empty;

    public List<decimal> Prices { get; set; } = new();
}