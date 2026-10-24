using InventoryManagementApp.Data;
using InventoryManagementApp.Models;
using InventoryManagementApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryDbContext _context;

        public ProductsController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StockBalances)
                    .ThenInclude(sb => sb.Location)
                        .ThenInclude(l => l.Warehouse)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StockBalances)
                    .ThenInclude(sb => sb.Location)
                        .ThenInclude(l => l.Warehouse)
                .Include(p => p.StockTransactions.OrderByDescending(t => t.CreatedAtUtc).Take(15))
                    .ThenInclude(t => t.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var model = new ProductCreateEditViewModel
            {
                CategoryOptions = await GetCategoryOptionsAsync(),
                LocationOptions = await GetLocationOptionsAsync()
            };
            return View(model);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateEditViewModel model)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == model.SKU.Trim()))
            {
                ModelState.AddModelError(nameof(model.SKU), "A product with this SKU already exists.");
            }

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    SKU = model.SKU.Trim().ToUpper(),
                    Barcode = model.Barcode?.Trim(),
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    CategoryId = model.CategoryId,
                    UnitCost = model.UnitCost,
                    SellingPrice = model.SellingPrice,
                    DefaultReorderPoint = model.DefaultReorderPoint,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // If initial location selected, create stock balance
                if (model.InitialLocationId.HasValue && model.InitialLocationId.Value > 0)
                {
                    var balance = new StockBalance
                    {
                        ProductId = product.Id,
                        LocationId = model.InitialLocationId.Value,
                        QuantityOnHand = model.InitialQuantity,
                        QuantityAllocated = 0,
                        ReorderPoint = product.DefaultReorderPoint,
                        LastStockTakeDate = DateTime.UtcNow
                    };
                    _context.StockBalances.Add(balance);

                    if (model.InitialQuantity > 0)
                    {
                        var transaction = new StockTransaction
                        {
                            ProductId = product.Id,
                            LocationId = model.InitialLocationId.Value,
                            QuantityChanged = model.InitialQuantity,
                            ResultingBalance = model.InitialQuantity,
                            TransactionType = TransactionType.Receipt,
                            ReferenceNumber = "INIT-" + product.SKU,
                            Notes = "Initial inventory balance on product creation",
                            CreatedBy = "System Admin",
                            CreatedAtUtc = DateTime.UtcNow
                        };
                        _context.StockTransactions.Add(transaction);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Product '{product.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            model.CategoryOptions = await GetCategoryOptionsAsync();
            model.LocationOptions = await GetLocationOptionsAsync();
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var model = new ProductCreateEditViewModel
            {
                Id = product.Id,
                SKU = product.SKU,
                Barcode = product.Barcode,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                UnitCost = product.UnitCost,
                SellingPrice = product.SellingPrice,
                DefaultReorderPoint = product.DefaultReorderPoint,
                CategoryOptions = await GetCategoryOptionsAsync()
            };

            return View(model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCreateEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (await _context.Products.AnyAsync(p => p.SKU == model.SKU.Trim() && p.Id != id))
            {
                ModelState.AddModelError(nameof(model.SKU), "Another product already uses this SKU.");
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return NotFound();

                product.SKU = model.SKU.Trim().ToUpper();
                product.Barcode = model.Barcode?.Trim();
                product.Name = model.Name.Trim();
                product.Description = model.Description?.Trim();
                product.CategoryId = model.CategoryId;
                product.UnitCost = model.UnitCost;
                product.SellingPrice = model.SellingPrice;
                product.DefaultReorderPoint = model.DefaultReorderPoint;

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product details updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            model.CategoryOptions = await GetCategoryOptionsAsync();
            return View(model);
        }

        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetLocationOptionsAsync()
        {
            return await _context.Locations
                .Include(l => l.Warehouse)
                .OrderBy(l => l.Warehouse.Name)
                .ThenBy(l => l.Aisle)
                .Select(l => new SelectListItem 
                { 
                    Value = l.Id.ToString(), 
                    Text = $"{l.Warehouse.Name} - {l.LocationCode}" 
                })
                .ToListAsync();
        }
    }
}
