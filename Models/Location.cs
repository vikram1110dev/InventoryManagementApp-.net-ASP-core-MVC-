using System.ComponentModel.DataAnnotations;

namespace InventoryManagementApp.Models
{
    public class Location
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        [Required, StringLength(20)]
        public string Aisle { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Shelf { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Bin { get; set; } = string.Empty;

        public decimal? MaxVolumeCapacity { get; set; }

        public string LocationCode => $"{Warehouse?.Code ?? "WH"}-{Aisle}-{Shelf}-{Bin}";

        // Navigation property
        public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
