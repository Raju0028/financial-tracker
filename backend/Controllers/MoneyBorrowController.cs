using FinancialTracker.Models;
using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoneyBorrowController : ControllerBase
{
    private readonly MoneyBorrowService _moneyBorrowService;

    public MoneyBorrowController(MoneyBorrowService moneyBorrowService)
    {
        _moneyBorrowService = moneyBorrowService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMoneyBorrow()
    {
        var borrows = await _moneyBorrowService.GetMoneyBorrowAsync();

        return Ok(borrows);
    }

    [HttpPost]
    public async Task<IActionResult> AddMoneyBorrow(
        [FromBody] MoneyBorrow moneyBorrow)
    {
        await _moneyBorrowService.AddMoneyBorrowAsync(moneyBorrow);

        return Ok(new
        {
            message = "Money borrowed successfully."
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMoneyBorrow(
    [FromQuery] string sheetName,
    [FromQuery] int rowNumber)
    {
        await _moneyBorrowService.DeleteMoneyBorrowAsync(
            sheetName,
            rowNumber);

        return Ok(new
        {
            message = "Money borrowed record deleted successfully."
        });
    }
}