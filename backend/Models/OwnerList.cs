namespace FinancialTracker.Models;

public class OwnerList
{
    public int RowNumber { get; set; }

    public string Date { get; set; } = string.Empty;

    public string Item { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public string Amount { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;
}