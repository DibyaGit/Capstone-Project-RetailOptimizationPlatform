using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailOptimizationPlatform.Core.Entities
{
    /// <summary>
    /// Represents a customer order in the retail system.
    /// </summary>
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters.")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order Date is required.")]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, 1000000.00, ErrorMessage = "Total Amount must be greater than zero.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        // --- NEW: Foreign Key linking the Order to a specific Inventory Item ---

        [Required(ErrorMessage = "You must select an inventory item.")]
        [Display(Name = "Purchased Item")]
        public int InventoryId { get; set; }

        // Navigation property for Entity Framework to build the SQL JOIN
        [ForeignKey("InventoryId")]
        public Inventory? Inventory { get; set; }
    }
}