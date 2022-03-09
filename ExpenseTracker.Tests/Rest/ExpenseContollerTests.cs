using System;
using NUnit.Framework;
using ExpenseTracker.Rest.Controllers;
using Moq;
using ExpenseTracker.Core.Services;
using AutoMapper;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Rest.Dtos;
using Microsoft.AspNetCore.Http;
using ExpenseTracker.Rest.Models;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Tests.Rest
{
    [TestFixture]
    public class ExpenseControllerTests
    {
        //private IMapper NewMapper => new MapperConfiguration(cfg => { cfg.AddProfile<MapperProfile>(); }).CreateMapper();
        private ExpenseDto _expenseDto = new ExpenseDto
        {
            Amount = 1,
            Id = 1,
            CategoryName = "SomeCategory",
            Date = DateTime.Now,
            Description = "Some Description"
        };

        private Expense _expense = new Expense(id: 1, amount: 100);

        private User _user = new User(1, "FirstUser@abc.com", "Firstname", "Lastname");

        private Category _category = new Category(1, "Category1");

        private Mock<IMapper> NewMapperMock => new Mock<IMapper>();

        private Mock<IExpenseService> NewExpenseServiceMock => new Mock<IExpenseService>();

        private Mock<ICategoryService> NewCategoryServiceMock => new Mock<ICategoryService>();

        private Mock<IUserService> NewUserServiceMock => new Mock<IUserService>();

        private Mock<TemplateService> NewTemplateServiceMock => new Mock<TemplateService>();

        [Test]
        public async Task Add_OnModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            ExpenseController sut = new ExpenseController(NewExpenseServiceMock.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object,
                                                            NewCategoryServiceMock.Object,
                                                            NewUserServiceMock.Object
                                                            );
            sut.ModelState.AddModelError("Amount", "Amount should be positive");

            // Act
            IActionResult response = await sut.Add(_expenseDto);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Add_OnGivenExpense_CreateAndReturnsExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Add(It.IsAny<Expense>())).ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            SetupDefaultCategoryServiceMock().Object,
                                                            SetupDefaultUserServiceMock().Object
                                                            );

            // Act
            IActionResult result = await sut.Add(_expenseDto);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status201Created, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status201Created, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto, appResponse.Data);
        }

        [Test]
        public void Add_OnGivenExpenseWithInvalidCategory_ThrowsException()
        {
            // Arrange
            ExpenseController sut = new ExpenseController(NewExpenseServiceMock.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object
                                                            );

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => sut.Add(_expenseDto));
        }

        [Test]
        public async Task Get_ReturnsExpenses()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<User>())).ReturnsAsync(new List<Expense> { _expense });
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);

            // Act
            var result = await sut.Get();

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto,((IEnumerable<ExpenseDto>)appResponse.Data).First());
        }

        [Test]
        public async Task Get_OnPaginationInputs_ReturnsExpenses()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.GetAll(It.IsAny<User>(), It.IsAny<Func<Expense, bool>>(),
                                            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Expense> { _expense });
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);

            // Act
            var result = await sut.Get(1, 1, true);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto, ((IEnumerable<ExpenseDto>)appResponse.Data).First());
        }

        [Test]
        public async Task Get_OnExpenseWithGivenIdNotPresent_ReturnsNotFound()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<int>()))
                .ReturnsAsync((Expense)null);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);

            // Act
            var result = await sut.Get(1);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Get_OnExpenseWithIdPresent_ReturnsExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<int>()))
                .ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);
            // Act
            var result = await sut.Get(1);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto, appResponse.Data);
        }

        [Test]
        public async Task Get_ReturnsExpenseForGivenCategory()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Expense> { _expense });
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);

            // Act
            var result = await sut.Get("CAtegory1");

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto, ((IEnumerable<ExpenseDto>)appResponse.Data).First());
        }

        [Test]
        public async Task Delete_DeletesAndReturnsExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Delete(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);

            // Act
            IActionResult result = await sut.Delete(1);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto, appResponse.Data);
        }

        [Test]
        public async Task Put_OnModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            ExpenseController sut = new ExpenseController(NewExpenseServiceMock.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object,
                                                            NewCategoryServiceMock.Object,
                                                            NewUserServiceMock.Object);
            sut.ModelState.AddModelError("Amount", "Should be greater than 0");

            // Act
            IActionResult response = await sut.Put(new ExpenseDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Put_ReturnsUpdatedExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Update(It.IsAny<Expense>()))
                            .ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);
            // Act
            IActionResult result = await sut.Put(new ExpenseDto());

            // Assert
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(_expenseDto, appResponse.Data);
        }

        [TestCase(1)]
        [TestCase(100)]
        [TestCase(0)]
        [TestCase(123)]
        public async Task GetCount_ReturnsExpenseCount(int expectedCount)
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.GetExpenseCount(_user))
                            .ReturnsAsync(expectedCount);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object,
                                                            NewCategoryServiceMock.Object,
                                                            SetupDefaultUserServiceMock().Object);

            // Act
            IActionResult response = await sut.GetCount();

            // Assert
            var objectResult = response as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            var appResponse = objectResult.Value as Response;
            Assert.AreEqual(StatusCodes.Status200OK, appResponse.StatusCode);
            Assert.AreEqual(expectedCount, appResponse.Data);
        }

        private Mock<IMapper> SetupDefaultMapperMock()
        {
            var mapperMock = NewMapperMock;
            mapperMock.Setup(x => x.Map<Expense>(It.IsAny<ExpenseDto>())).Returns(_expense);
            mapperMock.Setup(x => x.Map<ExpenseDto>(It.IsAny<Expense>())).Returns(_expenseDto);
            return mapperMock;
        }

        private Mock<IUserService> SetupDefaultUserServiceMock()
        {
            var userService = NewUserServiceMock;
            userService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(_user);
            return userService;
        }

        private Mock<ICategoryService> SetupDefaultCategoryServiceMock()
        { 
            var categoryService = NewCategoryServiceMock;
            categoryService.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(_category);
            return categoryService;
        }

        private void AssertExpense(ExpenseDto expectedExpense, ExpenseDto expenseToAssert)
        {
            Assert.AreEqual(expectedExpense.Id, expenseToAssert.Id);
            Assert.AreEqual(expectedExpense.Amount, expenseToAssert.Amount);
            Assert.AreEqual(expectedExpense.Date, expenseToAssert.Date);
            Assert.AreEqual(expectedExpense.Description, expenseToAssert.Description);
            Assert.AreEqual(expectedExpense.CategoryName, expenseToAssert.CategoryName);
        }
    }
}