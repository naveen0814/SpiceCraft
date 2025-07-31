using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.Controllers;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;

namespace SpiceCraft.Tests
{
    [TestFixture]
    public class CartItemControllerTests
    {
        private Mock<ICartItemService> _cartItemServiceMock;
        private CartItemController _controller;

        [SetUp]
        public void Setup()
        {
            _cartItemServiceMock = new Mock<ICartItemService>();
            _controller = new CartItemController(_cartItemServiceMock.Object);

            // Prepare HttpContext for user claims
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        private void SetUserId(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithUserCartItems()
        {
            // Arrange
            int userId = 101;
            SetUserId(userId);

            var items = new List<CartItemDto>
            {
                new CartItemDto { CartItemId = 1, CartId = 10, MenuId = 100, MenuItemName = "ItemA", MenuItemPrice = 50m, Quantity = 2, TotalPrice = 100m },
                new CartItemDto { CartItemId = 2, CartId = 10, MenuId = 101, MenuItemName = "ItemB", MenuItemPrice = 30m, Quantity = 1, TotalPrice = 30m }
            };
            _cartItemServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(items);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(items));
        }

  
        [Test]
        public async Task Get_ReturnsNotFound_WhenOwnedButNotFound()
        {
            // Arrange
            int userId = 103;
            SetUserId(userId);
            int cartItemId = 6;

            _cartItemServiceMock.Setup(s => s.IsCartItemOwnedByUserAsync(cartItemId, userId))
                                .ReturnsAsync(true);
            _cartItemServiceMock.Setup(s => s.GetByIdAsync(cartItemId))
                                .ReturnsAsync((CartItemDto?)null);

            // Act
            var result = await _controller.Get(cartItemId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart item not found."));
        }

        [Test]
        public async Task Get_ReturnsOk_WhenOwnedAndFound()
        {
            // Arrange
            int userId = 104;
            SetUserId(userId);
            int cartItemId = 7;

            var dto = new CartItemDto
            {
                CartItemId = cartItemId,
                CartId = 20,
                MenuId = 200,
                MenuItemName = "ItemC",
                MenuItemPrice = 75m,
                Quantity = 3,
                TotalPrice = 225m
            };
            _cartItemServiceMock.Setup(s => s.IsCartItemOwnedByUserAsync(cartItemId, userId))
                                .ReturnsAsync(true);
            _cartItemServiceMock.Setup(s => s.GetByIdAsync(cartItemId))
                                .ReturnsAsync(dto);

            // Act
            var result = await _controller.Get(cartItemId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            int userId = 105;
            SetUserId(userId);
            _controller.ModelState.AddModelError("CartId", "Required");

            var invalidDto = new CreateCartItemDto
            {
                // missing required fields
            };

            // Act
            var result = await _controller.Create(invalidDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }


        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenCartOwned()
        {
            // Arrange
            int userId = 107;
            SetUserId(userId);
            var dto = new CreateCartItemDto { CartId = 40, MenuId = 400, Quantity = 1 };
            var created = new CartItemDto
            {
                CartItemId = 8,
                CartId = dto.CartId,
                MenuId = dto.MenuId,
                MenuItemName = "ItemD",
                MenuItemPrice = 120m,
                Quantity = dto.Quantity,
                TotalPrice = 120m
            };

            _cartItemServiceMock.Setup(s => s.IsCartOwnedByUserAsync(dto.CartId, userId))
                                .ReturnsAsync(true);
            _cartItemServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(_controller.Get)));
            Assert.That(createdResult?.RouteValues?["id"], Is.EqualTo(created.CartItemId));
            Assert.That(createdResult?.Value, Is.EqualTo(created));
        }


        [Test]
        public async Task Update_ReturnsNotFound_WhenOwnedButNotFound()
        {
            // Arrange
            int userId = 109;
            SetUserId(userId);
            int cartItemId = 10;
            var dto = new UpdateCartItemDto { CartId = 60, MenuId = 600, Quantity = 3 };

            _cartItemServiceMock.Setup(s => s.IsCartItemOwnedByUserAsync(cartItemId, userId))
                                .ReturnsAsync(true);
            _cartItemServiceMock.Setup(s => s.UpdateAsync(cartItemId, dto))
                                .ReturnsAsync((CartItemDto?)null);

            // Act
            var result = await _controller.Update(cartItemId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart item not found."));
        }



        [Test]
        public async Task Delete_ReturnsNotFound_WhenOwnedButDeleteFails()
        {
            // Arrange
            int userId = 112;
            SetUserId(userId);
            int cartItemId = 13;

            _cartItemServiceMock.Setup(s => s.IsCartItemOwnedByUserAsync(cartItemId, userId))
                                .ReturnsAsync(true);
            _cartItemServiceMock.Setup(s => s.DeleteAsync(cartItemId))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(cartItemId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart item not found."));
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenOwnedAndDeleted()
        {
            // Arrange
            int userId = 113;
            SetUserId(userId);
            int cartItemId = 14;

            _cartItemServiceMock.Setup(s => s.IsCartItemOwnedByUserAsync(cartItemId, userId))
                                .ReturnsAsync(true);
            _cartItemServiceMock.Setup(s => s.DeleteAsync(cartItemId))
                                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(cartItemId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
