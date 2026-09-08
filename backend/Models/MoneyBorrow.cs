namespace FinancialTracker.Models;

public class MoneyBorrow
{
    public int RowNumber { get; set; }

    public string SheetName { get; set; } = string.Empty;

    public string Date { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Bank { get; set; } = string.Empty;

    public string Person { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;
}