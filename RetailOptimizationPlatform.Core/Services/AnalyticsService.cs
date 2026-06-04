using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;

namespace RetailOptimizationPlatform.Core.Services
{
    /// <summary>
    /// Analytics service implementation providing business logic for dashboard metrics and KPIs.
    /// Calculates real-time analytics from inventory and order data using SOLID principles.
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IOrderRepository _orderRepository;

        public AnalyticsService(
            IInventoryRepository inventoryRepository,
            IOrderRepository orderRepository)
        {
            _inventoryRepository = inventoryRepository;
            _orderRepository = orderRepository;
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var inventory = await _inventoryRepository.GetAllAsync();
            return inventory.Sum(i => i.Price * i.StockQuantity);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Count();
        }

        public async Task<int> GetLowStockItemsCountAsync(int threshold = 10)
        {
            var inventory = await _inventoryRepository.GetAllAsync();
            return inventory.Count(i => i.StockQuantity < threshold);
        }

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            if (!orders.Any())
                return 0;

            return orders.Average(o => o.TotalAmount);
        }

        public async Task<IEnumerable<TopSellingItemDto>> GetTopSellingItemsAsync(int topCount = 5)
        {
            var orders = await _orderRepository.GetAllAsync();
            var inventory = await _inventoryRepository.GetAllAsync();

            var topItems = orders
                .GroupBy(o => o.InventoryId)
                .Select(g => new TopSellingItemDto
                {
                    InventoryId = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => o.TotalAmount),
                    ItemName = inventory.FirstOrDefault(i => i.Id == g.Key)?.ItemName ?? "Unknown"
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(topCount)
                .ToList();

            return topItems;
        }

        public async Task<IEnumerable<SalesTrendDto>> GetSalesTrendAsync(int months = 12)
        {
            var orders = await _orderRepository.GetAllAsync();
            var cutoffDate = DateTime.UtcNow.AddMonths(-months);

            var salesTrend = orders
                .Where(o => o.OrderDate >= cutoffDate)
                .GroupBy(o => o.OrderDate.Year * 100 + o.OrderDate.Month)
                .Select(g => new
                {
                    Key = g.Key,
                    Month = new DateTime(g.Key / 100, g.Key % 100, 1).ToString("MMM-yy"),
                    MonthlySales = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Key)
                .Select(x => new SalesTrendDto
                {
                    Month = x.Month,
                    MonthlySales = x.MonthlySales,
                    OrderCount = x.OrderCount
                })
                .ToList();

            return salesTrend;
        }

        public async Task<IEnumerable<StockDistributionDto>> GetStockDistributionAsync()
        {
            var inventory = await _inventoryRepository.GetAllAsync();

            // Categorize items into buckets
            var distribution = inventory
                .GroupBy(i => CategorizeItem(i.ItemName))
                .Select(g => new StockDistributionDto
                {
                    Category = g.Key,
                    ItemCount = g.Count(),
                    TotalValue = g.Sum(i => i.Price * i.StockQuantity),
                    TotalStock = g.Sum(i => i.StockQuantity)
                })
                .OrderByDescending(x => x.TotalValue)
                .ToList();

            return distribution;
        }

        public async Task<IEnumerable<OrderStatusSummaryDto>> GetOrderStatusSummaryAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            var summary = new List<OrderStatusSummaryDto>
            {
                new OrderStatusSummaryDto { Status = "Total Orders", Count = orders.Count() },
                new OrderStatusSummaryDto { Status = "Recent (7 days)", Count = orders.Count(o => o.OrderDate >= DateTime.UtcNow.AddDays(-7)) },
                new OrderStatusSummaryDto { Status = "High Value (>₹5000)", Count = orders.Count(o => o.TotalAmount > 5000) },
                new OrderStatusSummaryDto { Status = "Low Value (<₹1000)", Count = orders.Count(o => o.TotalAmount < 1000) }
            };

            return summary;
        }

        public async Task<decimal> GetInventoryTurnoverRatioAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var inventory = await _inventoryRepository.GetAllAsync();

            if (!inventory.Any())
                return 0;

            // Simplified: Orders in last 30 days / Average inventory value
            var recentOrdersValue = orders
                .Where(o => o.OrderDate >= DateTime.UtcNow.AddDays(-30))
                .Sum(o => o.TotalAmount);

            var avgInventoryValue = inventory.Sum(i => i.Price * i.StockQuantity) / inventory.Count();

            if (avgInventoryValue == 0)
                return 0;

            return recentOrdersValue / avgInventoryValue;
        }

        /// <summary>
        /// Helper method to categorize items based on name patterns.
        /// </summary>
        private static string CategorizeItem(string itemName)
        {
            if (itemName.Contains("Keyboard", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("Mouse", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("Monitor", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("USB", StringComparison.OrdinalIgnoreCase))
                return "Electronics";

            if (itemName.Contains("Chair", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("Desk", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("Cabinet", StringComparison.OrdinalIgnoreCase))
                return "Furniture";

            if (itemName.Contains("Shirt", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("Pants", StringComparison.OrdinalIgnoreCase) ||
                itemName.Contains("Hat", StringComparison.OrdinalIgnoreCase))
                return "Apparel";

            return "Other";
        }
    }
}
