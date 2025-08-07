using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;
using LogiTrack.Data;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;

    public InventoryController(LogiTrackContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

   [HttpGet]
public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
{
    const string cacheKey = "inventoryList";

    if (!_cache.TryGetValue(cacheKey, out List<InventoryItem> inventory))
    {
        inventory = await _context.InventoryItems
                                  .Include(i => i.Order)
                                  .AsNoTracking()
                                  .ToListAsync();

        var cacheOptions = new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
};

        _cache.Set(cacheKey, inventory, cacheOptions);
    }

    return Ok(inventory);
}


    [HttpPost]
    public async Task<ActionResult<InventoryItem>> AddItem(InventoryItem item)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetInventory), new { id = item.ItemId }, item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
