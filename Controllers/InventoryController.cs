using InventoryManagementApp.Models;
using InventoryManagementApp.Models.ViewModels;
using InventoryManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementApp.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: /Inventory (Multi-criteria search & filter utility)
        public async Task<IActionResult> Index([FromQuery] InventorySearchViewModel criteria)
        {
            var model = await _inventoryService.SearchInventoryAsync(criteria);
            return View(model);
        }

        // GET: /Inventory/StockIn/{id}
        public async Task<IActionResult> StockIn(int id)
        {
            var model = await _inventoryService.GetStockOperationDetailsAsync(id, TransactionType.Receipt);
            if (model == null) return NotFound();
            return View("StockOperation", model);
        }

        // GET: /Inventory/StockOut/{id}
        public async Task<IActionResult> StockOut(int id)
        {
            var model = await _inventoryService.GetStockOperationDetailsAsync(id, TransactionType.Dispatch);
            if (model == null) return NotFound();
            return View("StockOperation", model);
        }

        // GET: /Inventory/Adjust/{id}
        public async Task<IActionResult> Adjust(int id)
        {
            var model = await _inventoryService.GetStockOperationDetailsAsync(id, TransactionType.Adjustment);
            if (model == null) return NotFound();
            model.Quantity = model.CurrentOnHand; // default to current count for adjustments
            return View("StockOperation", model);
        }

        // POST: /Inventory/ProcessOperation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOperation(StockOperationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("StockOperation", model);
            }

            var (success, message) = await _inventoryService.ProcessStockOperationAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View("StockOperation", model);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Inventory/Transactions
        public async Task<IActionResult> Transactions(string? search, TransactionType? type)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentType = type;
            var history = await _inventoryService.GetTransactionHistoryAsync(search, type);
            return View(history);
        }
    }
}
