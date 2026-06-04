using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using RetailOptimizationPlatform.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetailOptimizationPlatform.Tests.Controllers
{
    public class SuppliersControllerTests
    {
        private readonly Mock<ISupplierRepository> _mockRepo;
        private readonly SuppliersController _controller;

        public SuppliersControllerTests()
        {
            _mockRepo = new Mock<ISupplierRepository>();
            _controller = new SuppliersController(_mockRepo.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfSuppliers()
        {
            // Arrange
            var mockSuppliers = new List<Supplier>
            {
                new() { Id = 1, Name = "Supplier A", ContactPerson = "Contact A", Email = "a@a.com", PhoneNumber = "1", City = "City A", Country = "Country A" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(mockSuppliers);

            // Act
            var result = await _controller.Index(null!);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Supplier>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Index_WithSearchString_CallsSearchAsync()
        {
            // Arrange
            var searchString = "Supplier A";
            var mockSuppliers = new List<Supplier>
            {
                new() { Id = 1, Name = "Supplier A", ContactPerson = "Contact A", Email = "a@a.com", PhoneNumber = "1", City = "City A", Country = "Country A" }
            };
            _mockRepo.Setup(r => r.SearchAsync(searchString)).ReturnsAsync(mockSuppliers);

            // Act
            var result = await _controller.Index(searchString);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Supplier>>(viewResult.Model);
            Assert.Single(model);
            _mockRepo.Verify(r => r.SearchAsync(searchString), Times.Once);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithSupplier()
        {
            // Arrange
            var supplier = new Supplier { Id = 1, Name = "Supplier A", ContactPerson = "Contact A", Email = "a@a.com", PhoneNumber = "1", City = "City A", Country = "Country A" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Supplier>(viewResult.Model);
            Assert.Equal("Supplier A", model.Name);
        }
    }
}
