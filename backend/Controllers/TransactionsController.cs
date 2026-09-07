using FinancialTracker.Models;
using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionsController(
        TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    // GET recent transactions
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentTransactions()
    {
        var transactions =
            await _transactionService.GetRecentTransactionsAsync();

        return Ok(transactions);
    }

    // POST recent transaction
    [HttpPost("recent")]
    public async Task<IActionResult> AddRecentTransaction(
        [FromBody] Transaction transaction)
    {
        await _transactionService.AddRecentTransactionAsync(
            transaction);

        return Ok(new
        {
            message = "Recent transaction added successfully."
        });
    }

}