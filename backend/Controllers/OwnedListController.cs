using FinancialTracker.Models;
using FinancialTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnedListController : ControllerBase
{
    private readonly OwnedListService  _ownedListService;

    public OwnedListController(
        OwnedListService ownedListService)
    {
        _ownedListService = ownedListService;
    }

    // GET owned list
    [HttpGet("ownerlists")]
    public async Task<IActionResult> GetOwnerLists()
    {
        var ownerLists =
            await _ownedListService.GetOwnerListsAsync();

        return Ok(ownerLists);
    }


    // POST owned list
    [HttpPost("ownerlists")]
    public async Task<IActionResult> AddOwnerList(
        [FromBody] OwnerList ownerList)
    {
        await _ownedListService.AddOwnerListAsync(
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
        await _ownedListService.UpdateOwnerListAsync(
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
        await _ownedListService.DeleteOwnerListAsync(
            rowNumber);

        return Ok(new
        {
            message = "Owned list item deleted successfully."
        });
    }
}