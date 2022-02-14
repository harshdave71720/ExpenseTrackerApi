using System;
using NUnit.Framework;
using ExpenseTracker.Rest.Controllers;
using Moq;
using ExpenseTracker.Core.Services;
using AutoMapper;
using ExpenseTracker.Rest.MapperProfiles;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Rest.Dtos;

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

        private Mock<IMapper> NewMapperMock => new Mock<IMapper>();

        private Mock<IExpenseService> NewExpenseServiceMock => new Mock<IExpenseService>();

        private Mock<TemplateService> NewTemplateServiceMock => new Mock<TemplateService>();

        [Test]
        public async Task Add_OnModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            ExpenseController sut = new ExpenseController(NewExpenseServiceMock.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);
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
            expenseService.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Expense>(), It.IsAny<string>())).ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);

            // Act
            IActionResult result = await sut.Add(_expenseDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);

            var createdExpense = ((OkObjectResult)result).Value as ExpenseDto;
            Assert.IsNotNull(createdExpense);
            Assert.AreEqual(_expenseDto, createdExpense);
        }

        [Test]
        public async Task Get_ReturnsExpenses()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(new List<Expense> { _expense });
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);

            // Act
            var response = await sut.Get();
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var expenses = (IEnumerable<ExpenseDto>)((OkObjectResult)response).Value;
            Assert.AreEqual(1, expenses.Count());
            Assert.AreEqual(_expenseDto, expenses.First());
        }

        [Test]
        public async Task Get_OnPaginationInputs_ReturnsExpenses()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.GetAll(It.IsAny<string>(), It.IsAny<Func<Expense, bool>>(),
                                            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Expense> { _expense });
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);

            // Act
            var response = await sut.Get(1, 1, true);
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var expenses = (IEnumerable<ExpenseDto>)((OkObjectResult)response).Value;
            Assert.AreEqual(1, expenses.Count());
            Assert.AreEqual(_expenseDto, expenses.First());
        }

        [Test]
        public async Task Get_OnExpenseWithIdNotPresent_ReturnsNotFound()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Expense)null);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);

            // Act
            var response = await sut.Get(1);
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task Get_OnExpenseWithIdPresent_ReturnsExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);

            // Act
            var response = await sut.Get(1);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var returnedExpense = ((OkObjectResult)response).Value as ExpenseDto;
            Assert.IsNotNull(returnedExpense);
            Assert.AreEqual(_expenseDto, returnedExpense);
        }

        [Test]
        public async Task Get_OnCategoryNotExists_ReturnsNotFound()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Expense>)null);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);

            // Act
            var response = await sut.Get("CAtegory1");

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task Get_ReturnsExpenseForGivenCategory()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Expense>{ _expense });
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);

            // Act
            var response = await sut.Get("CAtegory1");

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var expenses = (IEnumerable<ExpenseDto>)((OkObjectResult)response).Value;
            Assert.AreEqual(1, expenses.Count());
            Assert.AreEqual(_expenseDto, expenses.First());
        }

        [Test]
        public async Task Delete_OnExpenseNotPresent_ReturnsNotFound()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((Expense)null);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);
            // Act
            IActionResult response = await sut.Delete(1);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task Delete_OnExpensePresent_DeletesAndReturnsExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);

            // Act
            IActionResult response = await sut.Delete(1);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var deletedExpense = (ExpenseDto)((OkObjectResult)response).Value;
            Assert.AreEqual(_expenseDto, deletedExpense);
        }

        [Test]
        public async Task Put_OnModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            ExpenseController sut = new ExpenseController(NewExpenseServiceMock.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);
            sut.ModelState.AddModelError("Amount", "Should be greater than 0");

            // Act
            IActionResult response = await sut.Put(new ExpenseDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Put_OnExpenseNotPresent_ReturnsNotFound()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Expense>(), It.IsAny<string>()))
                            .ReturnsAsync((Expense)null);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);
            // Act
            IActionResult response = await sut.Put(new ExpenseDto());

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task Put_OnExpensePresent_ReturnsUpdatedExpense()
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Expense>(), It.IsAny<string>()))
                            .ReturnsAsync(_expense);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            SetupDefaultMapperMock().Object);
            // Act
            IActionResult response = await sut.Put(new ExpenseDto());

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var updatedExpense = (ExpenseDto)((OkObjectResult)response).Value;
            Assert.AreEqual(_expenseDto, updatedExpense);
        }

        [TestCase(1)]
        [TestCase(100)]
        [TestCase(0)]
        [TestCase(123)]
        public async Task GetCount_ReturnsExpenseCount(int expectedCount)
        {
            // Arrange
            var expenseService = NewExpenseServiceMock;
            expenseService.Setup(x => x.GetExpenseCount(It.IsAny<string>()))
                            .ReturnsAsync(expectedCount);
            ExpenseController sut = new ExpenseController(expenseService.Object,
                                                            NewTemplateServiceMock.Object,
                                                            NewMapperMock.Object);

            // Act
            IActionResult response = await sut.GetCount();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            int count = (int)((OkObjectResult)response).Value;
            Assert.AreEqual(expectedCount, count);
        }

        private Mock<IMapper> SetupDefaultMapperMock()
        {
            var mapperMock = NewMapperMock;
            mapperMock.Setup(x => x.Map<Expense>(It.IsAny<ExpenseDto>())).Returns(_expense);
            mapperMock.Setup(x => x.Map<ExpenseDto>(It.IsAny<Expense>())).Returns(_expenseDto);
            return mapperMock;
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