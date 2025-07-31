using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.Controllers;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using System.Collections.Generic;
using System;

namespace SpiceCraft.Tests
{
    [TestFixture]
    public class DiscountControllerTests
    {
        private Mock<IDiscountService> _discountServiceMock;
        private DiscountController _controller;

        [SetUp]
        public void Setup()
        {
            _discountServiceMock = new Mock<IDiscountService>();
            _controller = new DiscountController(_discountServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithDiscountList()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var mockDiscounts = new List<DiscountDto>
            {
                new DiscountDto
                {
                    DiscountId = 1,
                    RestaurantId = 10,
                    RestaurantName = "R1",
                    MenuId = 100,
                    MenuItemName = "ItemA",
                    DiscountPercentage = 10m,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(5)
                },
                new DiscountDto
                {
                    DiscountId = 2,
                    RestaurantId = 20,
                    RestaurantName = "R2",
                    MenuId = 200,
                    MenuItemName = "ItemB",
                    DiscountPercentage = 15m,
                    StartDate = now.AddDays(-3),
                    EndDate = now.AddDays(7)
                }
            };
            _discountServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(mockDiscounts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(mockDiscounts));
        }

        [Test]
        public async Task Get_WhenDiscountExists_ReturnsOk()
        {
            // Arrange
            var id = 5;
            var now = DateTime.UtcNow;
            var dto = new DiscountDto
            {
                DiscountId = id,
                RestaurantId = 30,
                RestaurantName = "R3",
                MenuId = 300,
                MenuItemName = "ItemC",
                DiscountPercentage = 20m,
                StartDate = now.AddDays(-1),
                EndDate = now.AddDays(9)
            };
            _discountServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task Get_WhenDiscountNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 99;
            _discountServiceMock.Setup(s => s.GetByIdAsync(missingId)).ReturnsAsync((DiscountDto?)null);

            // Act
            var result = await _controller.Get(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Discount not found."));
        }

        [Test]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var createDto = new CreateDiscountDto
            {
                RestaurantId = 40,
                MenuId = 400,
                DiscountPercentage = 25m,
                StartDate = now,
                EndDate = now.AddDays(10)
            };
            var created = new DiscountDto
            {
                DiscountId = 7,
                RestaurantId = createDto.RestaurantId,
                RestaurantName = "R4",
                MenuId = createDto.MenuId,
                MenuItemName = "ItemD",
                DiscountPercentage = createDto.DiscountPercentage,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate
            };
            _discountServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(created);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(_controller.Get)));
            Assert.That(createdResult?.RouteValues?["id"], Is.EqualTo(created.DiscountId));
            Assert.That(createdResult?.Value, Is.EqualTo(created));
        }

        [Test]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("DiscountPercentage", "Required");
            var invalid = new CreateDiscountDto { /* missing fields */ };

            // Act
            var result = await _controller.Create(invalid);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_MismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var routeId = 5;
            var dto = new UpdateDiscountDto
            {
                DiscountId = 99,
                RestaurantId = 50,
                MenuId = 500,
                DiscountPercentage = 30m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            // Act
            var result = await _controller.Update(routeId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result as BadRequestObjectResult;
            Assert.That(bad?.Value, Is.EqualTo("Mismatched ID."));
        }

        [Test]
        public async Task Update_WhenDiscountExists_ReturnsOk()
        {
            // Arrange
            var id = 8;
            var now = DateTime.UtcNow;
            var dto = new UpdateDiscountDto
            {
                DiscountId = id,
                RestaurantId = 60,
                MenuId = 600,
                DiscountPercentage = 35m,
                StartDate = now,
                EndDate = now.AddDays(7)
            };
            var updated = new DiscountDto
            {
                DiscountId = id,
                RestaurantId = dto.RestaurantId,
                RestaurantName = "R6",
                MenuId = dto.MenuId,
                MenuItemName = "ItemF",
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            _discountServiceMock.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(updated));
        }

        [Test]
        public async Task Update_WhenDiscountNotFound_ReturnsNotFound()
        {
            // Arrange
            var id = 123;
            var dto = new UpdateDiscountDto
            {
                DiscountId = id,
                RestaurantId = 70,
                MenuId = 700,
                DiscountPercentage = 40m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(3)
            };
            _discountServiceMock.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync((DiscountDto?)null);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Discount not found."));
        }

        [Test]
        public async Task Delete_DiscountNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 456;
            _discountServiceMock.Setup(s => s.DeleteAsync(missingId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Discount not found."));
        }

        [Test]
        public async Task Delete_DiscountExists_ReturnsNoContent()
        {
            // Arrange
            var id = 9;
            _discountServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
