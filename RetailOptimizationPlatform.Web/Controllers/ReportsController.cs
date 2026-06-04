using Microsoft.AspNetCore.Mvc;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace RetailOptimizationPlatform.Web.Controllers
{
    /// <summary>
    /// Reports controller for generating business analytics and insights.
    /// Provides detailed views of sales trends, inventory analytics, and order analytics.
    /// Implements separation of concerns with analytics service layer.
    /// </summary>
    public class ReportsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IOrderRepository _orderRepository;

        public ReportsController(
            IAnalyticsService analyticsService,
            IInventoryRepository inventoryRepository,
            IOrderRepository orderRepository)
        {
            _analyticsService = analyticsService;
            _inventoryRepository = inventoryRepository;
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Sales Analytics Report - Shows trends, top items, and order metrics.
        /// </summary>
        public async Task<IActionResult> SalesAnalytics()
        {
            ViewData["Title"] = "Sales Analytics Report";

            var salesTrend = await _analyticsService.GetSalesTrendAsync(12);
            var topItems = await _analyticsService.GetTopSellingItemsAsync(10);
            var orderStatus = await _analyticsService.GetOrderStatusSummaryAsync();
            var totalOrders = await _analyticsService.GetTotalOrdersCountAsync();
            var avgOrderValue = await _analyticsService.GetAverageOrderValueAsync();
            var totalSales = salesTrend.Sum(s => (decimal)s.MonthlySales);

            ViewBag.SalesTrend = salesTrend;
            ViewBag.TopItems = topItems;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.AverageOrderValue = avgOrderValue;
            ViewBag.TotalSales = totalSales;

            return View();
        }

        /// <summary>
        /// Inventory Analytics Report - Shows stock levels, distribution, and valuation.
        /// </summary>
        public async Task<IActionResult> InventoryAnalytics()
        {
            ViewData["Title"] = "Inventory Analytics Report";

            var inventory = await _inventoryRepository.GetAllAsync();
            var stockDistribution = await _analyticsService.GetStockDistributionAsync();
            var totalValue = await _analyticsService.GetTotalInventoryValueAsync();
            var lowStockCount = await _analyticsService.GetLowStockItemsCountAsync();
            var turnoverRatio = await _analyticsService.GetInventoryTurnoverRatioAsync();

            // Inventory metrics
            var totalItems = inventory.Count();
            var averageValue = totalValue / (totalItems > 0 ? totalItems : 1);
            var totalStock = inventory.Sum(i => i.StockQuantity);
            var outOfStock = inventory.Count(i => i.StockQuantity == 0);

            ViewBag.Inventory = inventory;
            ViewBag.StockDistribution = stockDistribution;
            ViewBag.TotalValue = totalValue;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.TurnoverRatio = turnoverRatio;
            ViewBag.TotalItems = totalItems;
            ViewBag.AverageValue = averageValue;
            ViewBag.TotalStock = totalStock;
            ViewBag.OutOfStock = outOfStock;

            return View();
        }

        /// <summary>
        /// Order Analytics Report - Shows order patterns, customer metrics, and trends.
        /// </summary>
        public async Task<IActionResult> OrderAnalytics()
        {
            ViewData["Title"] = "Order Analytics Report";

            var orders = await _orderRepository.GetAllAsync();
            var orderCount = orders.Count();
            var totalOrderValue = orders.Sum(o => o.TotalAmount);
            var avgOrderValue = orderCount > 0 ? totalOrderValue / orderCount : 0;
            var topCustomers = orders
                .GroupBy(o => o.CustomerName)
                .Select(g => new RetailOptimizationPlatform.Core.Entities.TopCustomerDto { Customer = g.Key, OrderCount = g.Count(), Total = g.Sum(o => o.TotalAmount) })
                .OrderByDescending(x => x.Total)
                .Take(10)
                .ToList();

            var recentOrders = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10);

            var ordersByMonth = await _analyticsService.GetSalesTrendAsync(12);

            ViewBag.OrderCount = orderCount;
            ViewBag.TotalOrderValue = totalOrderValue;
            ViewBag.AverageOrderValue = avgOrderValue;
            ViewBag.TopCustomers = topCustomers;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.OrdersByMonth = ordersByMonth;

            return View();
        }

        /// <summary>
        /// Performance Dashboard - Executive summary of key metrics.
        /// </summary>
        public async Task<IActionResult> PerformanceDashboard()
        {
            ViewData["Title"] = "Performance Dashboard";

            var totalValue = await _analyticsService.GetTotalInventoryValueAsync();
            var totalOrders = await _analyticsService.GetTotalOrdersCountAsync();
            var lowStockItems = await _analyticsService.GetLowStockItemsCountAsync();
            var avgOrderValue = await _analyticsService.GetAverageOrderValueAsync();
            var turnoverRatio = await _analyticsService.GetInventoryTurnoverRatioAsync();

            ViewBag.TotalInventoryValue = totalValue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.LowStockItems = lowStockItems;
            ViewBag.AverageOrderValue = avgOrderValue;
            ViewBag.TurnoverRatio = turnoverRatio;

            return View();
        }

        /// <summary>
        /// Stored Procedure Revenue Report - Executes sp_GenerateRevenueReport via raw ADO.NET command.
        /// </summary>
        public async Task<IActionResult> StoredProcedureRevenueReport()
        {
            ViewData["Title"] = "Stored Procedure Revenue Report";
            var reportData = await _orderRepository.GetRevenueReportFromStoredProcedureAsync();
            return View(reportData);
        }
    }
}
