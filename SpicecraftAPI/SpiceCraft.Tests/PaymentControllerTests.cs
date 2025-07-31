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
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithPaymentList()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var mockPayments = new List<PaymentDto>
            {
                new PaymentDto
                {
                    PaymentId = 1,
                    OrderId = 100,
                    PaymentStatus = "Pending",
                    PaymentMode = "CreditCard",
                    TransactionId = "TXN123",
                    PaymentDate = now.AddDays(-1)
                },
                new PaymentDto
                {
                    PaymentId = 2,
                    OrderId = 101,
                    PaymentStatus = "Completed",
                    PaymentMode = "UPI",
                    TransactionId = "TXN456",
                    PaymentDate = now.AddHours(-5)
                }
            };
            _paymentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(mockPayments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(mockPayments));
        }

        [Test]
        public async Task Get_WhenPaymentExists_ReturnsOk()
        {
            // Arrange
            var paymentId = 5;
            var now = DateTime.UtcNow;
            var paymentDto = new PaymentDto
            {
                PaymentId = paymentId,
                OrderId = 200,
                PaymentStatus = "Completed",
                PaymentMode = "DebitCard",
                TransactionId = "TXN789",
                PaymentDate = now.AddMinutes(-30)
            };
            _paymentServiceMock.Setup(s => s.GetByIdAsync(paymentId)).ReturnsAsync(paymentDto);

            // Act
            var result = await _controller.Get(paymentId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(paymentDto));
        }

        [Test]
        public async Task Get_WhenPaymentNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 99;
            _paymentServiceMock.Setup(s => s.GetByIdAsync(missingId)).ReturnsAsync((PaymentDto?)null);

            // Act
            var result = await _controller.Get(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Payment not found."));
        }

        [Test]
        public async Task Create_ValidPayment_ReturnsCreatedAtAction()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var createDto = new CreatePaymentDto
            {
                OrderId = 300,
                PaymentStatus = "Pending",
                PaymentMode = "NetBanking",
                TransactionId = "TXN999",
                PaymentDate = now
            };
            var createdDto = new PaymentDto
            {
                PaymentId = 7,
                OrderId = createDto.OrderId,
                PaymentStatus = createDto.PaymentStatus,
                PaymentMode = createDto.PaymentMode,
                TransactionId = createDto.TransactionId,
                PaymentDate = createDto.PaymentDate
            };
            _paymentServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var created = result as CreatedAtActionResult;
            Assert.That(created?.ActionName, Is.EqualTo(nameof(_controller.Get)));
            Assert.That(created?.RouteValues?["id"], Is.EqualTo(createdDto.PaymentId));
            Assert.That(created?.Value, Is.EqualTo(createdDto));
        }

        [Test]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange: simulate invalid ModelState
            _controller.ModelState.AddModelError("PaymentStatus", "Required");
            var invalidDto = new CreatePaymentDto
            {
                // missing or invalid data
            };

            // Act
            var result = await _controller.Create(invalidDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_MismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var routeId = 5;
            var dto = new UpdatePaymentDto
            {
                PaymentId = 99,  // mismatch
                OrderId = 400,
                PaymentStatus = "Completed",
                PaymentMode = "Cash",
                TransactionId = "TXN111",
                PaymentDate = DateTime.UtcNow
            };

            // Act
            var result = await _controller.Update(routeId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result as BadRequestObjectResult;
            Assert.That(bad?.Value, Is.EqualTo("Mismatched ID."));
        }

        [Test]
        public async Task Update_WhenPaymentExists_ReturnsOk()
        {
            // Arrange
            var paymentId = 8;
            var now = DateTime.UtcNow.AddDays(-2);
            var updateDto = new UpdatePaymentDto
            {
                PaymentId = paymentId,
                OrderId = 500,
                PaymentStatus = "Completed",
                PaymentMode = "Wallet",
                TransactionId = "TXN222",
                PaymentDate = DateTime.UtcNow
            };
            var updatedDto = new PaymentDto
            {
                PaymentId = paymentId,
                OrderId = updateDto.OrderId,
                PaymentStatus = updateDto.PaymentStatus,
                PaymentMode = updateDto.PaymentMode,
                TransactionId = updateDto.TransactionId,
                PaymentDate = updateDto.PaymentDate
            };
            _paymentServiceMock.Setup(s => s.UpdateAsync(paymentId, updateDto)).ReturnsAsync(updatedDto);

            // Act
            var result = await _controller.Update(paymentId, updateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(updatedDto));
        }

        [Test]
        public async Task Update_WhenPaymentNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 123;
            var updateDto = new UpdatePaymentDto
            {
                PaymentId = missingId,
                OrderId = 600,
                PaymentStatus = "Failed",
                PaymentMode = "UPI",
                TransactionId = "TXN333",
                PaymentDate = DateTime.UtcNow
            };
            _paymentServiceMock.Setup(s => s.UpdateAsync(missingId, updateDto)).ReturnsAsync((PaymentDto?)null);

            // Act
            var result = await _controller.Update(missingId, updateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Payment not found."));
        }

        [Test]
        public async Task Delete_PaymentNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 456;
            _paymentServiceMock.Setup(s => s.DeleteAsync(missingId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Payment not found."));
        }

        [Test]
        public async Task Delete_PaymentExists_ReturnsNoContent()
        {
            // Arrange
            var paymentId = 9;
            _paymentServiceMock.Setup(s => s.DeleteAsync(paymentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(paymentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
