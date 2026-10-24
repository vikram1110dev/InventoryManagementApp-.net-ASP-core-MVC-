using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Barcode { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; } = 0.00m;

        public int DefaultReorderPoint { get; set; } = 10;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
