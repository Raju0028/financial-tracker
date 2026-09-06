using FinancialTracker.Models;
using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanController : ControllerBase
{
    private readonly LoanService _loanService;

    public LoanController(LoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLoans()
    {
        var loans = await _loanService.GetLoansAsync();

        return Ok(loans);
    }

    [HttpGet("repayments")]
    public async Task<IActionResult> GetLoanRepayments()
    {
        var repayments = await _loanService.GetLoanRepaymentsAsync();

        return Ok(repayments);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetLoanSummary()
    {
        var summary = await _loanService.GetLoanSummaryAsync();

        return Ok(summary);
    }

    [HttpPost]
    public async Task<IActionResult> AddLoan([FromBody] Loan loan)
    {
        await _loanService.AddLoanAsync(loan);

        return Ok(new
        {
            message = "Loan added successfully."
        });
    }

    [HttpPost("repayments")]
    public async Task<IActionResult> AddLoanRepayment(
        [FromBody] LoanRepayment repayment)
    {
        await _loanService.AddLoanRepaymentAsync(repayment);

        return Ok(new
        {
            message = "Repayment added successfully."
        });
    }
}