using System.Diagnostics;
using InventoryManagementApp.Models;
using InventoryManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IInventoryService inventoryService, ILogger<HomeController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _inventoryService.GetDashboardMetricsAsync();
            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
