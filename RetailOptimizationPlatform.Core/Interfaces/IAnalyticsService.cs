using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Core.Interfaces
{
    /// <summary>
    /// Analytics service interface for dashboard and reporting metrics.
    /// Implements business logic for calculating KPIs and generating insights.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Calculate total inventory value (Price * Quantity) across all items.
        /// </summary>
        Task<decimal> GetTotalInventoryValueAsync();

        /// <summary>
        /// Count total number of pending orders.
        /// </summary>
        Task<int> GetTotalOrdersCountAsync();

        /// <summary>
        /// Count inventory items with stock below threshold.
        /// </summary>
        Task<int> GetLowStockItemsCountAsync(int threshold = 10);

        /// <summary>
        /// Calculate average order value.
        /// </summary>
        Task<decimal> GetAverageOrderValueAsync();

        /// <summary>
        /// Get top N best-selling items.
        /// </summary>
        Task<IEnumerable<RetailOptimizationPlatform.Core.Entities.TopSellingItemDto>> GetTopSellingItemsAsync(int topCount = 5);

        /// <summary>
        /// Get sales trend data for specified period (in months).
        /// </summary>
        Task<IEnumerable<RetailOptimizationPlatform.Core.Entities.SalesTrendDto>> GetSalesTrendAsync(int months = 12);

        /// <summary>
        /// Get inventory distribution by category.
        /// </summary>
        Task<IEnumerable<RetailOptimizationPlatform.Core.Entities.StockDistributionDto>> GetStockDistributionAsync();

        /// <summary>
        /// Get orders by status for dashboard display.
        /// </summary>
        Task<IEnumerable<RetailOptimizationPlatform.Core.Entities.OrderStatusSummaryDto>> GetOrderStatusSummaryAsync();

        /// <summary>
        /// Calculate inventory turnover ratio.
        /// </summary>
        Task<decimal> GetInventoryTurnoverRatioAsync();
    }
}
