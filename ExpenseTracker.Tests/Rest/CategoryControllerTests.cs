using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Services;
using Moq;
using ExpenseTracker.Rest.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ExpenseTracker.Rest.Models;
using System.Threading.Tasks;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Tests.Rest
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private Category _category = new Category(1, "Category1");

        private CategoryDto _categoryDto = new CategoryDto() { Id = 1, Name = "Category1" };

        private User _user = new User("testUser@abc.com", "test", "User");

        private Mock<IUserService> NewUserServiceMock => new Mock<IUserService>();

        private Mock<ICategoryService> NewCategoryServiceMock => new Mock<ICategoryService>();

        private Mock<IMapper> NewMapperMock => new Mock<IMapper>();

        [Test]
        public void Constructor_WhenDependencyNull_ThrowsException()
        {
            // Act and Assert
            Assert.Throws<DependencyNullException>(() => new CategoryController(null, NewCategoryServiceMock.Object, NewUserServiceMock.Object));
            Assert.Throws<DependencyNullException>(() => new CategoryController(new Mock<IMapper>().Object, null, NewUserServiceMock.Object));
            Assert.Throws<DependencyNullException>(() => new CategoryController(new Mock<IMapper>().Object, NewCategoryServiceMock.Object, null));
        }

        [Test]
        public async Task Get_ReturnsCategories()
        {
            // Arrange
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Get(_user)).ReturnsAsync(new List<Category> { _category });
            var sut = new CategoryController(SetupDefaultMapper().Object, categoryService.Object, SetupDefaultUserService().Object);

            // Act
            var result = await sut.Get() as OkObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
            Assert.AreEqual(_categoryDto, ((IEnumerable<CategoryDto>)response.Data).First());
        }

        [Test]
        public void Get_WhenCategoryWithGivenNameNotExist_ThrowsException()
        {
            // Arrange
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync((Category)null);
            var sut = new CategoryController(SetupDefaultMapper().Object, categoryService.Object, SetupDefaultUserService().Object);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => sut.Get("UnknownCategoryName"));
        }

        [Test]
        public async Task Get_ReturnsCategoryWithGivenName()
        {
            // Arrange
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync(_category);
            var sut = new CategoryController(SetupDefaultMapper().Object, categoryService.Object, SetupDefaultUserService().Object);

            // Act
            OkObjectResult result = await sut.Get("Category1") as OkObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
            Assert.AreEqual(_categoryDto, response.Data);
        }

        [Test]
        public async Task Add_OnModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            var sut = new CategoryController(NewMapperMock.Object, NewCategoryServiceMock.Object, NewUserServiceMock.Object);
            sut.ModelState.AddModelError("Name", "Name field is required");

            // Act
            var result = await sut.Add(_categoryDto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Test]
        public async Task Add_CreatesAndReturnsCategory()
        {
            // Arrange
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Add(It.IsAny<Category>())).ReturnsAsync(_category);
            var mapper = SetupDefaultMapper();
            mapper.Setup(x => x.Map<Category>(It.IsAny<CategoryDto>())).Returns(_category);
            var sut = new CategoryController
                        (
                            mapper.Object, 
                            categoryService.Object, 
                            SetupDefaultUserService().Object
                        );

            // Act
            var result = await sut.Add(new CategoryDto() { Id = 1, Name = "Cat1"}) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            var response = result.Value as Response;
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual(_categoryDto, response.Data);
        }

        [Test]
        public async Task Delete_ReturnsDeletedCategory()
        {
            // Arrange
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Delete(_user, It.IsAny<string>())).ReturnsAsync(_category);
            var sut = new CategoryController
                        (
                            SetupDefaultMapper().Object,
                            categoryService.Object,
                            SetupDefaultUserService().Object
                        );

            // Act
            var result = await sut.Delete("Category1") as OkObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(_categoryDto, response.Data);
        }

        [Test]
        public async Task Put_OnModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            var sut = new CategoryController(NewMapperMock.Object, NewCategoryServiceMock.Object, NewUserServiceMock.Object);
            sut.ModelState.AddModelError("Name", "Name field is required");

            // Act
            var result = await sut.Put(_categoryDto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Test]
        public async Task Put_ReturnsUpdatedResult()
        {
            // Arrange
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync(_category);
            var mapper = SetupDefaultMapper();
            mapper.Setup(x => x.Map<Category>(It.IsAny<CategoryDto>())).Returns(_category);
            var sut = new CategoryController
                        (
                            mapper.Object,
                            categoryService.Object,
                            SetupDefaultUserService().Object
                        );

            // Act
            var result = await sut.Put(new CategoryDto() { Id = 1, Name = "Cat1" }) as OkObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(_categoryDto, response.Data);
        }

        private Mock<IUserService> SetupDefaultUserService()
        {
            var userService = NewUserServiceMock;
            userService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(_user);
            return userService;
        }

        private Mock<IMapper> SetupDefaultMapper()
        { 
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CategoryDto>(_category)).Returns(_categoryDto);
            return mapper;
        }
    }
}