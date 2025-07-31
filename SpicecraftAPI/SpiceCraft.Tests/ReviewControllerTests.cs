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
    public class ReviewControllerTests
    {
        private Mock<IReviewService> _reviewServiceMock;
        private ReviewController _controller;

        [SetUp]
        public void Setup()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _controller = new ReviewController(_reviewServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithReviewList()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var mockReviews = new List<ReviewDto>
            {
                new ReviewDto
                {
                    ReviewId = 1,
                    UserId = 100,
                    UserName = "Alice",
                    MenuId = 10,
                    MenuItemName = "Pasta",
                    Rating = 4,
                    Comment = "Tasty",
                    CreatedAt = now.AddDays(-1)
                },
                new ReviewDto
                {
                    ReviewId = 2,
                    UserId = 101,
                    UserName = "Bob",
                    MenuId = 20,
                    MenuItemName = "Burger",
                    Rating = 5,
                    Comment = "Excellent",
                    CreatedAt = now.AddHours(-5)
                }
            };
            _reviewServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(mockReviews);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(mockReviews));
        }

        [Test]
        public async Task Get_WhenReviewExists_ReturnsOk()
        {
            // Arrange
            var reviewId = 5;
            var now = DateTime.UtcNow;
            var reviewDto = new ReviewDto
            {
                ReviewId = reviewId,
                UserId = 200,
                UserName = "Charlie",
                MenuId = 15,
                MenuItemName = "Salad",
                Rating = 3,
                Comment = "Average",
                CreatedAt = now.AddMinutes(-30)
            };
            _reviewServiceMock.Setup(s => s.GetByIdAsync(reviewId)).ReturnsAsync(reviewDto);

            // Act
            var result = await _controller.Get(reviewId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(reviewDto));
        }

        [Test]
        public async Task Get_WhenReviewNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 99;
            _reviewServiceMock.Setup(s => s.GetByIdAsync(missingId)).ReturnsAsync((ReviewDto?)null);

            // Act
            var result = await _controller.Get(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            // Optionally check message:
            Assert.That(nf?.Value, Is.EqualTo("Review not found."));
        }

        [Test]
        public async Task Create_ValidReview_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateReviewDto
            {
                UserId = 300,
                MenuId = 30,
                Rating = 5,
                Comment = "Loved it"
            };
            var now = DateTime.UtcNow;
            var createdDto = new ReviewDto
            {
                ReviewId = 7,
                UserId = createDto.UserId,
                UserName = "TestUser",
                MenuId = createDto.MenuId,
                MenuItemName = "Pizza",
                Rating = createDto.Rating,
                Comment = createDto.Comment,
                CreatedAt = now
            };
            _reviewServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var created = result as CreatedAtActionResult;
            Assert.That(created?.ActionName, Is.EqualTo(nameof(_controller.Get)));
            Assert.That(created?.RouteValues?["id"], Is.EqualTo(createdDto.ReviewId));
            Assert.That(created?.Value, Is.EqualTo(createdDto));
        }

        [Test]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange: simulate invalid ModelState
            _controller.ModelState.AddModelError("Rating", "Required");
            var invalidDto = new CreateReviewDto
            {
                // missing required fields or invalid
            };

            // Act
            var result = await _controller.Create(invalidDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_WhenReviewExists_ReturnsOk()
        {
            // Arrange
            var reviewId = 8;
            var updateDto = new UpdateReviewDto
            {
                ReviewId = reviewId,
                UserId = 400,
                MenuId = 40,
                Rating = 2,
                Comment = "Not as expected"
            };
            var now = DateTime.UtcNow;
            var updatedDto = new ReviewDto
            {
                ReviewId = reviewId,
                UserId = updateDto.UserId,
                UserName = "User400",
                MenuId = updateDto.MenuId,
                MenuItemName = "Soup",
                Rating = updateDto.Rating,
                Comment = updateDto.Comment,
                CreatedAt = now.AddDays(-2)
            };
            _reviewServiceMock.Setup(s => s.UpdateAsync(reviewId, updateDto)).ReturnsAsync(updatedDto);

            // Act
            var result = await _controller.Update(reviewId, updateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(updatedDto));
        }

        [Test]
        public async Task Update_WhenReviewNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 123;
            var updateDto = new UpdateReviewDto
            {
                ReviewId = missingId,
                UserId = 500,
                MenuId = 50,
                Rating = 1,
                Comment = "Bad"
            };
            _reviewServiceMock.Setup(s => s.UpdateAsync(missingId, updateDto)).ReturnsAsync((ReviewDto?)null);

            // Act
            var result = await _controller.Update(missingId, updateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Review not found."));
        }

        [Test]
        public async Task Delete_ReviewNotFound_ReturnsNotFound()
        {
            // Arrange
            var missingId = 456;
            _reviewServiceMock.Setup(s => s.DeleteAsync(missingId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(missingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = result as NotFoundObjectResult;
            Assert.That(nf?.Value, Is.EqualTo("Review not found."));
        }

        [Test]
        public async Task Delete_ReviewExists_ReturnsNoContent()
        {
            // Arrange
            var reviewId = 9;
            _reviewServiceMock.Setup(s => s.DeleteAsync(reviewId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(reviewId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetByMenu_ReturnsOk_WithReviewList()
        {
            // Arrange
            var menuId = 50;
            var now = DateTime.UtcNow;
            var reviews = new List<ReviewDto>
            {
                new ReviewDto
                {
                    ReviewId = 11,
                    UserId = 500,
                    UserName = "User500",
                    MenuId = menuId,
                    MenuItemName = "Sandwich",
                    Rating = 4,
                    Comment = "Nice",
                    CreatedAt = now.AddHours(-3)
                }
            };
            _reviewServiceMock.Setup(s => s.GetByMenuIdAsync(menuId)).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetByMenu(menuId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(reviews));
        }

        [Test]
        public async Task GetByUser_ReturnsOk_WithReviewList()
        {
            // Arrange
            var userId = 600;
            var now = DateTime.UtcNow;
            var reviews = new List<ReviewDto>
            {
                new ReviewDto
                {
                    ReviewId = 12,
                    UserId = userId,
                    UserName = "User600",
                    MenuId = 60,
                    MenuItemName = "Steak",
                    Rating = 5,
                    Comment = "Great",
                    CreatedAt = now.AddDays(-1)
                }
            };
            _reviewServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetByUser(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(reviews));
        }
    }
}
