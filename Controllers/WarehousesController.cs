using InventoryManagementApp.Data;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly InventoryDbContext _context;

        public WarehousesController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: Warehouses
        public async Task<IActionResult> Index()
        {
            var warehouses = await _context.Warehouses
                .Include(w => w.Locations)
                    .ThenInclude(l => l.StockBalances)
                .OrderBy(w => w.Name)
                .ToListAsync();

            return View(warehouses);
        }

        // GET: Warehouses/CreateLocation
        public async Task<IActionResult> CreateLocation(int? warehouseId)
        {
            ViewBag.WarehouseList = await _context.Warehouses
                .Where(w => w.IsActive)
                .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = $"{w.Name} ({w.Code})" })
                .ToListAsync();

            var location = new Location();
            if (warehouseId.HasValue) location.WarehouseId = warehouseId.Value;
            return View(location);
        }

        // POST: Warehouses/CreateLocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(Location location)
        {
            var exists = await _context.Locations.AnyAsync(l => 
                l.WarehouseId == location.WarehouseId && 
                l.Aisle == location.Aisle.Trim().ToUpper() && 
                l.Shelf == location.Shelf.Trim().ToUpper() && 
                l.Bin == location.Bin.Trim().ToUpper());

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This exact Location (Aisle-Shelf-Bin) already exists in this Warehouse.");
            }

            if (ModelState.IsValid)
            {
                location.Aisle = location.Aisle.Trim().ToUpper();
                location.Shelf = location.Shelf.Trim().ToUpper();
                location.Bin = location.Bin.Trim().ToUpper();

                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Location '{location.Aisle}-{location.Shelf}-{location.Bin}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.WarehouseList = await _context.Warehouses
                .Where(w => w.IsActive)
                .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = $"{w.Name} ({w.Code})" })
                .ToListAsync();

            return View(location);
        }
    }
}
