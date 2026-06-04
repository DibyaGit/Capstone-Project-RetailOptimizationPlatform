using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailOptimizationPlatform.Api.Controllers;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RetailOptimizationPlatform.Tests.Controllers
{
    public class InventoryApiControllerTests
    {
        private readonly Mock<IInventoryRepository> _mockRepo;
        private readonly InventoryApiController _controller;

        // Constructor acts as the "Setup" for all tests
        public InventoryApiControllerTests()
        {
            _mockRepo = new Mock<IInventoryRepository>();
            _controller = new InventoryApiController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetInventory_ReturnsOkResult_WithListOfInventory()
        {
            // 1. Arrange
            var mockData = new List<Inventory>
            {
                new Inventory { Id = 1, ItemName = "Test Item 1", Sku = "TST-001", StockQuantity = 10, Price = 9.99M }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockData);

            // 2. Act
            var result = await _controller.GetInventory();

            // 3. Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Inventory>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<Inventory>>(okResult.Value);
            var items = (List<Inventory>)list;
            Assert.Single(items); // Verifies exactly 1 item was returned
        }

        [Fact]
        public async Task GetInventoryById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // 1. Arrange: Simulate the database returning null for ID 99
            _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Inventory)null!);

            // 2. Act
            var result = await _controller.GetInventory(99);

            // 3. Assert: Verify the controller caught the null and returned a 404 Not Found
            var actionResult = Assert.IsType<ActionResult<Inventory>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostInventory_ReturnsCreatedAtAction_WhenItemIsValid()
        {
            // 1. Arrange: Create a valid fake item
            var newItem = new Inventory { Id = 2, ItemName = "New Item", Sku = "NEW-002", StockQuantity = 5, Price = 15.00M };

            // Tell the mock repository to just pretend it completed the AddAsync task
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Inventory>())).Returns(Task.CompletedTask);

            // 2. Act
            var result = await _controller.PostInventory(newItem);

            // 3. Assert: Verify it returns a 201 Created status with the correct route values
            var actionResult = Assert.IsType<ActionResult<Inventory>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            Assert.Equal("GetInventory", createdResult.ActionName);
            var returnedItem = Assert.IsType<Inventory>(createdResult.Value);
            Assert.Equal(2, returnedItem.Id);
        }
    }
}