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
    public class CartControllerTests
    {
        private Mock<ICartService> _cartServiceMock;
        private CartController _controller;

        [SetUp]
        public void Setup()
        {
            _cartServiceMock = new Mock<ICartService>();
            _controller = new CartController(_cartServiceMock.Object);

            // Prepare HttpContext with a default ClaimsPrincipal; tests will override user ID per scenario
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // Helper to set current user ID in HttpContext.User
        private void SetUserId(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;
        }

        [Test]
        public async Task GetMyCart_ReturnsOk_WithListOfCartsForCurrentUser()
        {
            // Arrange
            int userId = 42;
            SetUserId(userId);

            var now = DateTime.UtcNow;
            var carts = new List<CartDto>
            {
                new CartDto { CartId = 1, UserId = userId, UserName = "User42", CreatedAt = now.AddDays(-1) },
                new CartDto { CartId = 2, UserId = userId, UserName = "User42", CreatedAt = now.AddHours(-5) }
            };
            _cartServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(carts);

            // Act
            var result = await _controller.GetMyCart();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(carts));
        }

        [Test]
        public async Task Get_ReturnsNotFound_WhenCartDoesNotExist()
        {
            // Arrange
            int userId = 10;
            SetUserId(userId);

            int cartId = 99;
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync((CartDto?)null);

            // Act
            var result = await _controller.Get(cartId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart not found."));
        }



        [Test]
        public async Task Get_ReturnsOk_WhenCartBelongsToUser()
        {
            // Arrange
            int userId = 10;
            SetUserId(userId);

            int cartId = 1;
            var cartDto = new CartDto { CartId = cartId, UserId = userId, UserName = "User10", CreatedAt = DateTime.UtcNow };
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.Get(cartId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(cartDto));
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenUserAlreadyHasCart()
        {
            // Arrange
            int userId = 20;
            SetUserId(userId);

            var existingCarts = new List<CartDto> { new CartDto { CartId = 5, UserId = userId, UserName = "User20", CreatedAt = DateTime.UtcNow } };
            _cartServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(existingCarts);

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result as BadRequestObjectResult;
            Assert.That(bad?.Value, Is.EqualTo("User already has a cart."));
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenUserHasNoCart()
        {
            // Arrange
            int userId = 30;
            SetUserId(userId);

            _cartServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(new List<CartDto>()); // no existing
            var createdCart = new CartDto { CartId = 7, UserId = userId, UserName = "User30", CreatedAt = DateTime.UtcNow };
            _cartServiceMock.Setup(s => s.CreateAsync(It.Is<CreateCartDto>(dto => dto.UserId == userId)))
                            .ReturnsAsync(createdCart);

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var created = result as CreatedAtActionResult;
            Assert.That(created?.ActionName, Is.EqualTo(nameof(_controller.Get)));
            Assert.That(created?.RouteValues?["id"], Is.EqualTo(createdCart.CartId));
            Assert.That(created?.Value, Is.EqualTo(createdCart));
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            int userId = 40;
            SetUserId(userId);

            int routeId = 5;
            var dto = new UpdateCartDto { CartId = 99, UserId = userId };

            // Act
            var result = await _controller.Update(routeId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result as BadRequestObjectResult;
            Assert.That(bad?.Value, Is.EqualTo("Mismatched Cart ID."));
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenCartDoesNotExist()
        {
            // Arrange
            int userId = 50;
            SetUserId(userId);

            int cartId = 8;
            var dto = new UpdateCartDto { CartId = cartId, UserId = userId };
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync((CartDto?)null);

            // Act
            var result = await _controller.Update(cartId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart not found."));
        }


        [Test]
        public async Task Update_ReturnsOk_WhenCartBelongsToUser()
        {
            // Arrange
            int userId = 70;
            SetUserId(userId);

            int cartId = 10;
            var existingCart = new CartDto { CartId = cartId, UserId = userId, UserName = "User70", CreatedAt = DateTime.UtcNow };
            var dto = new UpdateCartDto { CartId = cartId, UserId = userId };
            var updatedCart = new CartDto { CartId = cartId, UserId = userId, UserName = "User70", CreatedAt = DateTime.UtcNow };
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync(existingCart);
            _cartServiceMock.Setup(s => s.UpdateAsync(cartId, dto)).ReturnsAsync(updatedCart);

            // Act
            var result = await _controller.Update(cartId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(updatedCart));
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenCartDoesNotExist()
        {
            // Arrange
            int userId = 80;
            SetUserId(userId);

            int cartId = 11;
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync((CartDto?)null);

            // Act
            var result = await _controller.Delete(cartId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart not found."));
        }



        [Test]
        public async Task Delete_ReturnsNoContent_WhenCartBelongsToUser_AndDeleted()
        {
            // Arrange
            int userId = 100;
            SetUserId(userId);

            int cartId = 13;
            var existingCart = new CartDto { CartId = cartId, UserId = userId, UserName = "User100", CreatedAt = DateTime.UtcNow };
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync(existingCart);
            _cartServiceMock.Setup(s => s.DeleteAsync(cartId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(cartId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenCartBelongsToUser_ButDeleteFails()
        {
            // Arrange
            int userId = 110;
            SetUserId(userId);

            int cartId = 14;
            var existingCart = new CartDto { CartId = cartId, UserId = userId, UserName = "User110", CreatedAt = DateTime.UtcNow };
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync(existingCart);
            _cartServiceMock.Setup(s => s.DeleteAsync(cartId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(cartId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart not found."));
        }

        [Test]
        public async Task GetCartItems_ReturnsNotFound_WhenCartDoesNotExist()
        {
            // Arrange
            int userId = 120;
            SetUserId(userId);

            int cartId = 15;
            _cartServiceMock.Setup(s => s.GetByIdAsync(cartId)).ReturnsAsync((CartDto?)null);

            // Act
            var result = await _controller.GetCartItems(cartId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Cart not found."));
        }
    }

 


    
        
    
}
