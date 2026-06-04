using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailOptimizationPlatform.Core.Entities
{
    /// <summary>
    /// Represents a physical product in the retail inventory system.
    /// </summary>
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Item Name is required.")]
        [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters.")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 20 characters.")]
        public string Sku { get; set; } = string.Empty;

        [Required]
        [Range(0, 10000, ErrorMessage = "Stock quantity cannot be negative.")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be greater than zero.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal Price { get; set; }
    }
}