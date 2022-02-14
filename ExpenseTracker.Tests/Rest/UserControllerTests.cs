using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ExpenseTracker.Rest.Controllers;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Services;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Tests.Rest
{
    [TestFixture]
    internal class UserControllerTests
    {
        private User _user = new User(1, "abc@xyz.com", "abc", "def");
        private UserDto _userDto = new UserDto()
        { 
            Email = "abc@xyz.com",
            FirstName = "abc",
            LastName = "def"
        };

        [Test]
        public async Task Get_OnUserNotExists_ReturnsNotFound()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((User)null);
            UserController sut = new UserController(userService.Object, new Mock<IMapper>().Object);

            // Act
            IActionResult response = await sut.Get(_user.Email);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task Get_ReturnsUser()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(_user);
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<UserDto>(_user)).Returns(_userDto);
            UserController sut = new UserController(userService.Object, mapper.Object);

            // Act
            IActionResult response = await sut.Get(_user.Email);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            var user = ((OkObjectResult)response).Value as UserDto;
            Assert.AreEqual(_userDto, user);
        }

        [Test]
        public async Task Add_OnModelError_ReturnsBadRequest()
        {
            // Arrange
            UserController sut = new UserController(new Mock<IUserService>().Object, new Mock<IMapper>().Object);
            sut.ModelState.AddModelError("FirstName", "FirstName is required.");

            // Act
            IActionResult response = await sut.Add(_userDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Add_OnUserAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Add(_user)).ReturnsAsync((User)null);
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<User>(_userDto)).Returns(_user);
            UserController sut = new UserController(userService.Object, mapper.Object);

            // Act
            IActionResult response = await sut.Add(_userDto);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(response);
        }

        [Test]
        public async Task Add_ReturnAddUser()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Add(_user)).ReturnsAsync(_user);
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<User>(_userDto)).Returns(_user);
            mapper.Setup(x => x.Map<UserDto>(_user)).Returns(_userDto);
            UserController sut = new UserController(userService.Object, mapper.Object);

            // Act
            IActionResult response = await sut.Add(_userDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            var returnedUser = ((OkObjectResult)response).Value as UserDto;
            Assert.AreEqual(_userDto, returnedUser);
        }
    }
}
