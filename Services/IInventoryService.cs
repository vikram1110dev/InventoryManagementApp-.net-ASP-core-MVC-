using InventoryManagementApp.Models;
using InventoryManagementApp.Models.ViewModels;

namespace InventoryManagementApp.Services
{
    public interface IInventoryService
    {
        Task<DashboardViewModel> GetDashboardMetricsAsync();
        Task<InventorySearchViewModel> SearchInventoryAsync(InventorySearchViewModel criteria);
        Task<StockOperationViewModel?> GetStockOperationDetailsAsync(int stockBalanceId, TransactionType type);
        Task<(bool Success, string Message)> ProcessStockOperationAsync(StockOperationViewModel model);
        Task<List<StockTransaction>> GetTransactionHistoryAsync(string? search = null, TransactionType? type = null, int limit = 100);
    }
}
