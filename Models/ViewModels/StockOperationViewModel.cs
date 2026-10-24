using System.ComponentModel.DataAnnotations;
using InventoryManagementApp.Models;

namespace InventoryManagementApp.Models.ViewModels
{
    public class StockOperationViewModel
    {
        public int StockBalanceId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public int LocationId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public int CurrentOnHand { get; set; }
        public int CurrentAllocated { get; set; }
        public int AvailableQuantity => Math.Max(0, CurrentOnHand - CurrentAllocated);

        [Required]
        [Range(1, 100000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        public TransactionType OperationType { get; set; }

        [Required(ErrorMessage = "Reference number is required (e.g. PO-1234, SO-5678, ADJ-001).")]
        [StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Operator name is required for audit logs.")]
        [StringLength(100)]
        public string OperatorName { get; set; } = "Vikram Kumar";
    }
}
