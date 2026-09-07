using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;
using FinancialTracker.Models;

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

    [HttpPost]
    public async Task<IActionResult> AddMonthlyInvestment(
    [FromBody] AddMonthlyInvestment request)
    {
        await _monthlyInvestmentService
            .AddMonthlyInvestmentAsync(request);

        return Ok(new
        {
            message = "Monthly investment added successfully."
        });
    }

    [HttpGet("prices")]
    public async Task<IActionResult> GetExistingPrices(
    [FromQuery] string month,
    [FromQuery] string expense)
    {
        var prices = await _monthlyInvestmentService
            .GetExistingPricesAsync(month, expense);

        return Ok(prices);
    }
}