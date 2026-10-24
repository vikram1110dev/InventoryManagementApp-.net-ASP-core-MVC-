using System.ComponentModel.DataAnnotations;

namespace InventoryManagementApp.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? AddressLine { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
