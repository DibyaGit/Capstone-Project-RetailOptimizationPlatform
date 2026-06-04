using Microsoft.EntityFrameworkCore;
using RetailOptimizationPlatform.Core.Entities;

namespace RetailOptimizationPlatform.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. DATABASE NORMALIZATION: Enforce Unique SKU
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.Sku)
                .IsUnique();
            // 2. DATA INTEGRITY: Enforce Check Constraints at the SQL level (Updated for .NET 8)
            modelBuilder.Entity<Inventory>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Inventory_StockQuantity", "[StockQuantity] >= 0");
                    t.HasCheckConstraint("CK_Inventory_Price", "[Price] >= 0");
                });

            // 3. EXPLICIT RELATIONSHIP: Map the Foreign Key
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Inventory)
                .WithMany()
                .HasForeignKey(o => o.InventoryId)
                .OnDelete(DeleteBehavior.Cascade); // Automatically deletes associated customer orders when an inventory item is deleted

            // Notify EF Core that the Orders table has an AFTER INSERT trigger to prevent DML OUTPUT clause failures
            modelBuilder.Entity<Order>()
                .ToTable(tb => tb.HasTrigger("trg_AutoDecrementStock"));

            // 4. SEED DATA: Provide initial records for the presentation
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, ItemName = "Wireless Mouse", Sku = "MOUSE-001", StockQuantity = 50, Price = 25.99m },
                new Inventory { Id = 2, ItemName = "Mechanical Keyboard", Sku = "KEY-002", StockQuantity = 30, Price = 89.50m },
                new Inventory { Id = 3, ItemName = "USB-C Monitor", Sku = "MON-003", StockQuantity = 15, Price = 299.99m },
                new Inventory { Id = 4, ItemName = "Ergonomic Chair", Sku = "FURN-004", StockQuantity = 5, Price = 150.00m }
            );

            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Logitech Corp", ContactPerson = "Markus Senn", Email = "markus@logitech.com", PhoneNumber = "+1-555-0199", City = "Newark", State = "NJ", Country = "USA", LeadTimeDays = 5, PaymentTerms = "Net 30", CreditLimit = 50000m, IsActive = true, Rating = 4.8m, CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), LastUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Supplier { Id = 2, Name = "Keychron Inc", ContactPerson = "Nick Chen", Email = "nick@keychron.com", PhoneNumber = "+852-2345-6789", City = "Hong Kong", State = "", Country = "Hong Kong", LeadTimeDays = 12, PaymentTerms = "Net 15", CreditLimit = 30000m, IsActive = true, Rating = 4.5m, CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), LastUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Supplier { Id = 3, Name = "Dell Technologies", ContactPerson = "Sarah Connor", Email = "sarah@dell.com", PhoneNumber = "+1-800-456-3355", City = "Round Rock", State = "TX", Country = "USA", LeadTimeDays = 7, PaymentTerms = "Net 45", CreditLimit = 100000m, IsActive = true, Rating = 4.7m, CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), LastUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, CustomerName = "John Doe", OrderDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc), TotalAmount = 51.98m, InventoryId = 1 },
                new Order { Id = 2, CustomerName = "Jane Smith", OrderDate = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc), TotalAmount = 89.50m, InventoryId = 2 },
                new Order { Id = 3, CustomerName = "Bob Johnson", OrderDate = new DateTime(2026, 5, 5, 0, 0, 0, DateTimeKind.Utc), TotalAmount = 299.99m, InventoryId = 3 },
                new Order { Id = 4, CustomerName = "Alice Brown", OrderDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc), TotalAmount = 150.00m, InventoryId = 4 }
            );
        }
    }
}