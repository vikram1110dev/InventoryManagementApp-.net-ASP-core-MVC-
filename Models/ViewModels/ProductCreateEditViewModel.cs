using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagementApp.Models.ViewModels
{
    public class ProductCreateEditViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Barcode { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Please select a Category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Unit cost must be a valid non-negative number.")]
        [Display(Name = "Unit Cost ($)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Selling price must be a valid non-negative number.")]
        [Display(Name = "Selling Price ($)")]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Reorder point must be 0 or greater.")]
        [Display(Name = "Default Reorder Point")]
        public int DefaultReorderPoint { get; set; } = 10;

        [Display(Name = "Initial Stock Warehouse Location")]
        public int? InitialLocationId { get; set; }

        [Display(Name = "Initial Quantity")]
        [Range(0, 50000)]
        public int InitialQuantity { get; set; } = 0;

        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> LocationOptions { get; set; } = new();
    }
}
