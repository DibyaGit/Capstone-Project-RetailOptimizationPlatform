using Microsoft.AspNetCore.Mvc;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using RetailOptimizationPlatform.Web.Models;

namespace RetailOptimizationPlatform.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        // Injecting the analytics service
        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Fetch real analytics data from the service
            decimal totalValue = await _analyticsService.GetTotalInventoryValueAsync();
            int totalOrders = await _analyticsService.GetTotalOrdersCountAsync();
            int lowStockCount = await _analyticsService.GetLowStockItemsCountAsync();
            decimal avgOrderValue = await _analyticsService.GetAverageOrderValueAsync();

            // 2. Send the real numbers to the View using ViewBag
            ViewBag.TotalValue = totalValue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.LowStockItems = lowStockCount;
            ViewBag.AverageOrderValue = avgOrderValue;

            // 3. Get sales trend data (real)
            var salesTrend = await _analyticsService.GetSalesTrendAsync(6);
            if (salesTrend.Any())
            {
                ViewBag.SalesMonths = salesTrend.Select(s => (string)s.Month).ToArray();
                ViewBag.SalesValues = salesTrend.Select(s => (decimal)s.MonthlySales).ToArray();
            }
            else
            {
                ViewBag.SalesMonths = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
                ViewBag.SalesValues = new[] { 0m, 0m, 0m, 0m, 0m, 0m };
            }

            // 4. Get stock distribution data (real)
            var stockDist = await _analyticsService.GetStockDistributionAsync();
            if (stockDist.Any())
            {
                ViewBag.StockCategories = stockDist.Select(s => (string)s.Category).ToArray();
                ViewBag.StockValues = stockDist.Select(s => (int)s.ItemCount).ToArray();
            }
            else
            {
                ViewBag.StockCategories = new[] { "Electronics", "Apparel", "Home & Garden" };
                ViewBag.StockValues = new[] { 0, 0, 0 };
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}