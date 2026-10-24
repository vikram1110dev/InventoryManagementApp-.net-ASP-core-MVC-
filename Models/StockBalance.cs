using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementApp.Models
{
    public class StockBalance
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public int QuantityOnHand { get; set; } = 0;
        public int QuantityAllocated { get; set; } = 0;
        public int ReorderPoint { get; set; } = 10;

        public DateTime? LastStockTakeDate { get; set; }

        [NotMapped]
        public int AvailableQuantity => Math.Max(0, QuantityOnHand - QuantityAllocated);

        [NotMapped]
        public bool IsLowStock => QuantityOnHand <= ReorderPoint && QuantityOnHand > 0;

        [NotMapped]
        public bool IsOutOfStock => QuantityOnHand <= 0;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
