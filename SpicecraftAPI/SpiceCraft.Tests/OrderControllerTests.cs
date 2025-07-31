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
    public class OrderControllerTests
    {
        private Mock<IOrderService> _orderServiceMock;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _controller = new OrderController(_orderServiceMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        private void SetUser(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _controller.ControllerContext.HttpContext.User = principal;
        }

        [Test]
        public async Task GetAll_AsAdmin_ReturnsOkWithOrders()
        {
            // Arrange
            SetUser(1, "Admin");
            var now = DateTime.UtcNow;
            var orders = new List<OrderDto>
            {
                new OrderDto {
                    OrderId = 1, UserId = 10, UserName="U10",
                    RestaurantId=100, RestaurantName="R100",
                    TotalAmount=200m, OrderStatus="Pending", PaymentStatus="Unpaid",
                    PaymentMethod="Card", ShippingAddress="Addr",
                    CreatedAt=now.AddDays(-1), DeliveryPartnerId=null, DeliveryPartnerName=null,
                    OrderItems = new List<OrderItemDto>()
                }
            };
            _orderServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(orders));
        }

        [Test]
        public async Task GetAll_NotAdmin_StillReturnsOkBecauseAttributeNotEnforcedHere()
        {
            // Arrange: even non-admin, the method returns Ok in unit test (auth attribute not enforced)
            SetUser(2, "User");
            var orders = new List<OrderDto>();
            _orderServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Get_WhenExists_ReturnsOk()
        {
            // Arrange
            int id = 5;
            var dto = new OrderDto
            {
                OrderId = id,
                UserId = 20,
                UserName = "U20",
                RestaurantId = 200,
                RestaurantName = "R200",
                TotalAmount = 150m,
                OrderStatus = "Confirmed",
                PaymentStatus = "Paid",
                PaymentMethod = "UPI",
                ShippingAddress = "Addr2",
                CreatedAt = DateTime.UtcNow,
                DeliveryPartnerId = 300,
                DeliveryPartnerName = "DP",
                OrderItems = new List<OrderItemDto>()
            };
            _orderServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task Get_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            int missingId = 99;
            _orderServiceMock.Setup(s => s.GetByIdAsync(missingId)).ReturnsAsync((OrderDto?)null);

            // Act
            var result = await _controller.Get(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Order not found."));
        }

        [Test]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("TotalAmount", "Required");
            var dto = new CreateOrderDto();

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Create_Valid_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                UserId = 30,
                RestaurantId = 300,
                TotalAmount = 250m,
                OrderStatus = "New",
                PaymentStatus = "Pending",
                PaymentMethod = "Cash",
                ShippingAddress = "Addr3"
            };
            var created = new OrderDto
            {
                OrderId = 7,
                UserId = dto.UserId,
                UserName = "U30",
                RestaurantId = dto.RestaurantId,
                RestaurantName = "R300",
                TotalAmount = dto.TotalAmount,
                OrderStatus = dto.OrderStatus,
                PaymentStatus = dto.PaymentStatus,
                PaymentMethod = dto.PaymentMethod,
                ShippingAddress = dto.ShippingAddress,
                CreatedAt = DateTime.UtcNow,
                DeliveryPartnerId = null,
                DeliveryPartnerName = null,
                OrderItems = new List<OrderItemDto>()
            };
            _orderServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var cr = result as CreatedAtActionResult;
            Assert.That(cr?.ActionName, Is.EqualTo(nameof(_controller.Get)));
            Assert.That(cr?.RouteValues?["id"], Is.EqualTo(created.OrderId));
            Assert.That(cr?.Value, Is.EqualTo(created));
        }

        [Test]
        public async Task Update_MismatchedId_ReturnsBadRequest()
        {
            // Arrange
            int routeId = 5;
            var dto = new UpdateOrderDto
            {
                OrderId = 99,
                UserId = 40,
                RestaurantId = 400,
                TotalAmount = 100m,
                OrderStatus = "X",
                PaymentStatus = "Y",
                PaymentMethod = "M",
                ShippingAddress = "Addr"
            };
            // Act
            var result = await _controller.Update(routeId, dto);
            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result as BadRequestObjectResult;
            Assert.That(bad?.Value, Is.EqualTo("Mismatched ID."));
        }

        [Test]
        public async Task Update_WhenExists_ReturnsOk()
        {
            // Arrange
            int id = 8;
            var dto = new UpdateOrderDto
            {
                OrderId = id,
                UserId = 50,
                RestaurantId = 500,
                TotalAmount = 300m,
                OrderStatus = "Shipped",
                PaymentStatus = "Paid",
                PaymentMethod = "Card",
                ShippingAddress = "Addr4"
            };
            var updated = new OrderDto
            {
                OrderId = id,
                UserId = dto.UserId,
                UserName = "U50",
                RestaurantId = dto.RestaurantId,
                RestaurantName = "R500",
                TotalAmount = dto.TotalAmount,
                OrderStatus = dto.OrderStatus,
                PaymentStatus = dto.PaymentStatus,
                PaymentMethod = dto.PaymentMethod,
                ShippingAddress = dto.ShippingAddress,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DeliveryPartnerId = 600,
                DeliveryPartnerName = "DP",
                OrderItems = new List<OrderItemDto>()
            };
            _orderServiceMock.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(updated));
        }

        [Test]
        public async Task Update_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = 123;
            var dto = new UpdateOrderDto
            {
                OrderId = id,
                UserId = 60,
                RestaurantId = 600,
                TotalAmount = 150m,
                OrderStatus = "Delivered",
                PaymentStatus = "Paid",
                PaymentMethod = "UPI",
                ShippingAddress = "Addr5"
            };
            _orderServiceMock.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync((OrderDto?)null);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Order not found."));
        }

        [Test]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            // Arrange
            int id = 456;
            _orderServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Order not found."));
        }

        [Test]
        public async Task Delete_Exists_ReturnsNoContent()
        {
            // Arrange
            int id = 9;
            _orderServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PlaceOrderFromCart_NoUserClaim_ReturnsUnauthorized()
        {
            // Arrange: no user claim set
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            // Act
            var result = await _controller.PlaceOrderFromCart();
            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
            var un = result as UnauthorizedObjectResult;
            Assert.That(un?.Value, Is.EqualTo("No user context."));
        }

        



        [Test]
        public async Task UpdateOrderStatus_OrderNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = 90;
            _orderServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((OrderDto?)null);

            // Act
            var result = await _controller.UpdateOrderStatus(id, new UpdateOrderStatusDto { OrderStatus = "X" });

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Order not found."));
        }

        [Test]
        public async Task UpdateOrderStatus_UpdateFails_ReturnsBadRequest()
        {
            // Arrange
            int id = 91;
            var existing = new OrderDto
            {
                OrderId = id,
                UserId = 10,
                UserName = "U",
                RestaurantId = 20,
                RestaurantName = "R",
                TotalAmount = 100m,
                OrderStatus = "Old",
                PaymentStatus = "Paid",
                PaymentMethod = "Card",
                ShippingAddress = "Addr",
                CreatedAt = DateTime.UtcNow,
                DeliveryPartnerId = null,
                DeliveryPartnerName = null,
                OrderItems = new List<OrderItemDto>()
            };
            _orderServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(existing);
            _orderServiceMock.Setup(s => s.UpdateOrderStatusAsync(id, "New")).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateOrderStatus(id, new UpdateOrderStatusDto { OrderStatus = "New" });

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result as BadRequestObjectResult;
            Assert.That(bad?.Value, Is.EqualTo("Could not update order status."));
        }

        [Test]
        public async Task UpdateOrderStatus_Succeeds_ReturnsOk()
        {
            // Arrange
            int id = 92;
            var existing = new OrderDto
            {
                OrderId = id,
                UserId = 11,
                UserName = "U11",
                RestaurantId = 21,
                RestaurantName = "R21",
                TotalAmount = 120m,
                OrderStatus = "Old",
                PaymentStatus = "Paid",
                PaymentMethod = "Card",
                ShippingAddress = "Addr6",
                CreatedAt = DateTime.UtcNow,
                DeliveryPartnerId = 300,
                DeliveryPartnerName = "DP",
                OrderItems = new List<OrderItemDto>()
            };
            _orderServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(existing);
            _orderServiceMock.Setup(s => s.UpdateOrderStatusAsync(id, "Delivered")).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrderStatus(id, new UpdateOrderStatusDto { OrderStatus = "Delivered" });

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo("Order status updated."));
        }

        [Test]
        public async Task GetOrdersForCurrentUser_NoUserClaim_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            // Act
            var result = await _controller.GetOrdersForCurrentUser();
            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task GetOrdersForCurrentUser_NoOrders_ReturnsNotFound()
        {
            // Arrange
            int userId = 120;
            SetUser(userId, "User");
            _orderServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(new List<OrderDto>());

            // Act
            var result = await _controller.GetOrdersForCurrentUser();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("No orders found for this user."));
        }

        [Test]
        public async Task GetOrdersForCurrentUser_WithOrders_ReturnsOk()
        {
            // Arrange
            int userId = 121;
            SetUser(userId, "User");
            var list = new List<OrderDto> {
                new OrderDto {
                    OrderId=201, UserId=userId, UserName="U121",
                    RestaurantId=301, RestaurantName="R301",
                    TotalAmount=180m, OrderStatus="Pending", PaymentStatus="Paid",
                    PaymentMethod="UPI", ShippingAddress="Addr7", CreatedAt=DateTime.UtcNow,
                    DeliveryPartnerId=null, DeliveryPartnerName=null,
                    OrderItems=new List<OrderItemDto>()
                }
            };
            _orderServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetOrdersForCurrentUser();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(list));
        }

        [Test]
        public async Task GetOrdersForCurrentRestaurant_NoUserClaim_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            // Act
            var result = await _controller.GetOrdersForCurrentRestaurant();
            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task GetOrdersForCurrentRestaurant_NoOrders_ReturnsNotFound()
        {
            // Arrange
            int restId = 130;
            SetUser(restId, "Restaurant");
            _orderServiceMock.Setup(s => s.GetOrdersForRestaurantAsync(restId)).ReturnsAsync(new List<OrderDto>());

            // Act
            var result = await _controller.GetOrdersForCurrentRestaurant();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("No orders found for this restaurant."));
        }

        [Test]
        public async Task GetOrdersForCurrentRestaurant_WithOrders_ReturnsOk()
        {
            // Arrange
            int restId = 131;
            SetUser(restId, "Restaurant");
            var list = new List<OrderDto> {
                new OrderDto {
                    OrderId=202, UserId=50, UserName="U50",
                    RestaurantId=restId, RestaurantName="R131",
                    TotalAmount=220m, OrderStatus="Pending", PaymentStatus="Unpaid",
                    PaymentMethod="Cash", ShippingAddress="Addr8", CreatedAt=DateTime.UtcNow,
                    DeliveryPartnerId=null, DeliveryPartnerName=null,
                    OrderItems=new List<OrderItemDto>()
                }
            };
            _orderServiceMock.Setup(s => s.GetOrdersForRestaurantAsync(restId)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetOrdersForCurrentRestaurant();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(list));
        }
    }
}
