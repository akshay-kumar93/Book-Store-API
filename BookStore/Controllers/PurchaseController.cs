using BookStore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Policy = "BuyerPolicy")]
[ApiController]
[Route("api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly BookStoreContext _context;

    public PurchaseController(BookStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _context.Purchases.Where(p => p.BuyerId == userId).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Purchase>> GetPurchase(int id)
    {
        var purchase = await _context.Purchases.FindAsync(id);

        if (purchase == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (purchase.BuyerId != userId)
        {
            return Forbid();
        }

        return purchase;
    }

    [HttpPost]
    public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        purchase.BuyerId = userId;

        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchase);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePurchase(int id)
    {
        var purchase = await _context.Purchases.FindAsync(id);
        if (purchase == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (purchase.BuyerId != userId)
        {
            return Forbid();
        }

        _context.Purchases.Remove(purchase);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
