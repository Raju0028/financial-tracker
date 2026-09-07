namespace FinancialTracker.Models;

public class MonthlyPaymentSettings
{
    public string SheetName { get; set; } = string.Empty;

    public Dictionary<string, int> Months { get; set; } = new();

    public Dictionary<string, string> Expenses { get; set; } = new();
}