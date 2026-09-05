using FinancialTracker.Models;
using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly GoogleSheetsService _googleSheetsService;

    public TransactionsController(
        GoogleSheetsService googleSheetsService)
    {
        _googleSheetsService = googleSheetsService;
    }


    // GET transactions
    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions =
            await _googleSheetsService.GetTransactionsAsync();

        return Ok(transactions);
    }


    // GET owned list
    [HttpGet("ownerlists")]
    public async Task<IActionResult> GetOwnerLists()
    {
        var ownerLists =
            await _googleSheetsService.GetOwnerListsAsync();

        return Ok(ownerLists);
    }


    // POST owned list
    [HttpPost("ownerlists")]
    public async Task<IActionResult> AddOwnerList(
        [FromBody] OwnerList ownerList)
    {
        await _googleSheetsService.AddOwnerListAsync(
            ownerList);

        return Ok(new
        {
            message = "Owned list item added successfully."
        });
    }


    // PUT owned list
    [HttpPut("ownerlists/{rowNumber}")]
    public async Task<IActionResult> UpdateOwnerList(
        int rowNumber,
        [FromBody] OwnerList ownerList)
    {
        await _googleSheetsService.UpdateOwnerListAsync(
            rowNumber,
            ownerList);

        return Ok(new
        {
            message = "Owned list item updated successfully."
        });
    }


    // DELETE owned list
    [HttpDelete("ownerlists/{rowNumber}")]
    public async Task<IActionResult> DeleteOwnerList(
        int rowNumber)
    {
        await _googleSheetsService.DeleteOwnerListAsync(
            rowNumber);

        return Ok(new
        {
            message = "Owned list item deleted successfully."
        });
    }
}