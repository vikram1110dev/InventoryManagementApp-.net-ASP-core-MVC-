using System.ComponentModel.DataAnnotations;

namespace InventoryManagementApp.Models
{
    public class StockTransaction
    {
        public long Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public int QuantityChanged { get; set; } // + for receipt/in, - for dispatch/out

        public int ResultingBalance { get; set; }

        public TransactionType TransactionType { get; set; }

        [Required, StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required, StringLength(100)]
        public string CreatedBy { get; set; } = "Warehouse Operator";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
