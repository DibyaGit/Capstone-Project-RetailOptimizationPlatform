using Moq;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using RetailOptimizationPlatform.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetailOptimizationPlatform.Tests.Services
{
    public class AnalyticsServiceTests
    {
        private readonly Mock<IInventoryRepository> _mockInventoryRepo;
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly AnalyticsService _service;

        public AnalyticsServiceTests()
        {
            _mockInventoryRepo = new Mock<IInventoryRepository>();
            _mockOrderRepo = new Mock<IOrderRepository>();
            _service = new AnalyticsService(_mockInventoryRepo.Object, _mockOrderRepo.Object);
        }

        [Fact]
        public async Task GetTotalInventoryValueAsync_CalculatesCorrectValue()
        {
            // Arrange
            var mockInventory = new List<Inventory>
            {
                new() { Id = 1, ItemName = "Item 1", Price = 10.00m, StockQuantity = 5, Sku = "SKU-1" },
                new() { Id = 2, ItemName = "Item 2", Price = 25.00m, StockQuantity = 2, Sku = "SKU-2" }
            };
            _mockInventoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(mockInventory);

            // Act
            var result = await _service.GetTotalInventoryValueAsync();

            // Assert
            Assert.Equal(100.00m, result); // (10 * 5) + (25 * 2) = 50 + 50 = 100
        }

        [Fact]
        public async Task GetLowStockItemsCountAsync_FiltersCorrectly()
        {
            // Arrange
            var mockInventory = new List<Inventory>
            {
                new() { Id = 1, ItemName = "Low Stock", StockQuantity = 3, Sku = "SKU-1" },
                new() { Id = 2, ItemName = "High Stock", StockQuantity = 50, Sku = "SKU-2" },
                new() { Id = 3, ItemName = "Low Stock 2", StockQuantity = 9, Sku = "SKU-3" }
            };
            _mockInventoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(mockInventory);

            // Act
            var result = await _service.GetLowStockItemsCountAsync(10);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetAverageOrderValueAsync_ReturnsZeroWhenNoOrders()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Order>());

            // Act
            var result = await _service.GetAverageOrderValueAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetAverageOrderValueAsync_CalculatesAverageCorrectly()
        {
            // Arrange
            var mockOrders = new List<Order>
            {
                new() { Id = 1, TotalAmount = 100m, CustomerName = "A", InventoryId = 1 },
                new() { Id = 2, TotalAmount = 200m, CustomerName = "B", InventoryId = 2 }
            };
            _mockOrderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(mockOrders);

            // Act
            var result = await _service.GetAverageOrderValueAsync();

            // Assert
            Assert.Equal(150.00m, result);
        }
    }
}
