using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Core.Repositories;
using Moq;
using System.Threading.Tasks;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    internal class UserServiceTests
    {
        [Test]
        public void Instanciation_OnRepositoryNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(null));
        }

        [Test]
        public void Get_OnEmailNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            IUserService sut = new UserService(new Mock<IUserRepository>().Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("    "));
        }

        [Test]
        public async Task Get_OnUserExists_ReturnsUser()
        {
            // Arrange
            User user = new User(1, "abc@xyz.com", "abc", "def");
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(user);
            IUserService sut = new UserService(userRepo.Object);

            // Act
            User returnedUser = await sut.Get(user.Email);

            // Assert
            Assert.AreEqual(user, returnedUser);
        }

        [Test]
        public void Add_OnUserNull_ThrowsException()
        {
            // Arrage
            IUserService sut = new UserService(new Mock<IUserRepository>().Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(null));
        }

        [Test]
        public async Task Add_OnUserAlreadyExists_ReturnsNull()
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(x => x.Exists(It.IsAny<string>())).ReturnsAsync(true);
            IUserService sut = new UserService(userRepo.Object);

            // Act
            User addedUser = await sut.Add(new User(1, "abc@xyz.com", "abc", "def"));

            // Assert
            Assert.IsNull(addedUser);
        }

        [Test]
        public async Task Add_ReturnsAddedUser()
        {
            // Arrange
            User user = new User(1, "abc@xyz.com", "abc", "def");
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(x => x.Exists(It.IsAny<string>())).ReturnsAsync(false);
            userRepo.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync(user);
            IUserService sut = new UserService(userRepo.Object);

            // Act
            var addedUser = await sut.Add(new User(1, "abc@xyz.com", "abc", "def"));

            // Assert
            Assert.AreEqual(user, addedUser);
        }
    }
}
