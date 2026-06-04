using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailOptimizationPlatform.Core.Entities
{
    /// <summary>
    /// Represents a supplier providing inventory items to the retail system.
    /// Enables supply chain management and vendor tracking capabilities.
    /// </summary>
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(150, ErrorMessage = "Supplier Name cannot exceed 150 characters.")]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Person is required.")]
        [StringLength(100, ErrorMessage = "Contact Person cannot exceed 100 characters.")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(20, ErrorMessage = "Phone Number cannot exceed 20 characters.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "State/Province cannot exceed 100 characters.")]
        [Display(Name = "State/Province")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters.")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Range(0.01, 100, ErrorMessage = "Lead Time (days) must be between 1 and 100.")]
        [Display(Name = "Lead Time (Days)")]
        public int LeadTimeDays { get; set; } = 7;

        [Required(ErrorMessage = "Payment Terms are required.")]
        [StringLength(50, ErrorMessage = "Payment Terms cannot exceed 50 characters.")]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; } = "Net 30";

        [Range(0, 1000000, ErrorMessage = "Credit Limit must be non-negative.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Credit Limit (₹)")]
        public decimal CreditLimit { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Rating")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        [Column(TypeName = "decimal(3,1)")]
        public decimal Rating { get; set; } = 5.0m;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
