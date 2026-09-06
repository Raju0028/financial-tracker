namespace FinancialTracker.Models;

public class Loan
{
	public int RowNumber { get; set; }

	public string Date { get; set; } = string.Empty;

	public decimal LoanAmount { get; set; }

	public string Duration { get; set; } = string.Empty;

	public string From { get; set; } = string.Empty;

	public decimal TotalLoan { get; set; }

    public string Status { get; set; } = string.Empty;
}