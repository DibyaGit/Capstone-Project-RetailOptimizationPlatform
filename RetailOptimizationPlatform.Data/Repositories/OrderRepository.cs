using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;

namespace RetailOptimizationPlatform.Data.Repositories
{
    public class OrderRepository(ApplicationDbContext context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await context.Orders
                .Include(o => o.Inventory)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await context.Orders
                .Include(o => o.Inventory)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new KeyNotFoundException($"Order with ID {id} was not found.");
        }

        public async Task AddAsync(Order order)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await context.Orders.FindAsync(id)
                ?? throw new KeyNotFoundException($"Cannot delete. Order with ID {id} was not found.");

            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }

        // --- NEW METHOD ADDED HERE ---
        // Fulfills the requirement for "Raw SQL Joins" bypassing LINQ
        public async Task<IEnumerable<dynamic>> GetOrderSummariesWithRawSqlAsync()
        {
            var summaries = new List<dynamic>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                // Raw SQL JOIN between Orders and Inventories
                command.CommandText = @"
                    SELECT o.Id AS OrderId, o.CustomerName, o.TotalAmount, i.ItemName 
                    FROM Orders o
                    INNER JOIN Inventories i ON o.InventoryId = i.Id";

                context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        summaries.Add(new
                        {
                            OrderId = result.GetInt32(0),
                            CustomerName = result.GetString(1),
                            TotalAmount = result.GetDecimal(2),
                            ItemName = result.GetString(3)
                        });
                    }
                }
            }
            return summaries;
        }

        // Fulfills the requirement for "Stored Procedure" execution bypassing LINQ
        public async Task<IEnumerable<RevenueReportDto>> GetRevenueReportFromStoredProcedureAsync()
        {
            var report = new List<RevenueReportDto>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_GenerateRevenueReport";

                context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        report.Add(new RevenueReportDto
                        {
                            ItemName = result.IsDBNull(0) ? string.Empty : result.GetString(0),
                            Sku = result.IsDBNull(1) ? string.Empty : result.GetString(1),
                            TotalUnitsSold = result.IsDBNull(2) ? 0 : result.GetInt32(2),
                            TotalRevenueGenerated = result.IsDBNull(3) ? 0m : result.GetDecimal(3)
                        });
                    }
                }
            }
            return report;
        }
    }
}