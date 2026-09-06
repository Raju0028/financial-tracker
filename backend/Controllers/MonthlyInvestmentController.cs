using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyInvestmentController : ControllerBase
{
    private readonly MonthlyInvestmentService _monthlyInvestmentService;

    public MonthlyInvestmentController(
        MonthlyInvestmentService monthlyInvestmentService)
    {
        _monthlyInvestmentService = monthlyInvestmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlyInvestment()
    {
        var monthlyInvestment =
            await _monthlyInvestmentService.GetMonthlyInvestmentAsync();

        return Ok(monthlyInvestment);
    }
}