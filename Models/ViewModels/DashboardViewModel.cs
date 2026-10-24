using InventoryManagementApp.Models;

namespace InventoryManagementApp.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProductsCount { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int TotalWarehousesCount { get; set; }

        public List<StockTransaction> RecentTransactions { get; set; } = new();
        public List<StockBalance> LowStockAlerts { get; set; } = new();
        public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    }
}
