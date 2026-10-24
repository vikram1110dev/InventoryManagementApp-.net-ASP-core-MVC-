using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagementApp.Models.ViewModels
{
    public class InventorySearchViewModel
    {
        // Search Criteria Filters
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? WarehouseId { get; set; }
        public StockStatusFilter Status { get; set; } = StockStatusFilter.All;

        // Select lists for dropdown filters
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> WarehouseOptions { get; set; } = new();

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // Results
        public List<InventoryItemRowViewModel> Results { get; set; } = new();
    }

    public class InventoryItemRowViewModel
    {
        public int StockBalanceId { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public decimal UnitCost { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityOnHand { get; set; }
        public int QuantityAllocated { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReorderPoint { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
    }
}
