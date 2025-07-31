using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.Controllers;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using System.Collections.Generic;
using SpicecraftAPI.Models;

namespace SpiceCraft.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithUserList()
        {
            // Arrange
            var mockUsers = new List<UserDto>
            {
                new UserDto { UserId = 1, Name = "Alice" },
                new UserDto { UserId = 2, Name = "Bob" }
            };
            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(mockUsers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok?.Value, Is.Not.Null);
            Assert.That(ok?.Value, Is.EqualTo(mockUsers));
        }



    

        [Test]
        public async Task Create_ValidUser_ReturnsCreatedAtAction()
        {
            var createDto = new CreateUserDto
            {
                Name = "John",
                Email = "john@example.com",
                PasswordHash = "pass",
                PhoneNumber = "1234567890",
                Address = "Somewhere",
                Gender = "Male",
                Role = UserRole.User
            };

            var createdUser = new UserDto { UserId = 10, Name = "John" };
            _userServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdUser);

            var result = await _controller.Create(createDto);

            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var created = result as CreatedAtActionResult;
            Assert.That(created?.Value, Is.EqualTo(createdUser));
        }


       
    }
}