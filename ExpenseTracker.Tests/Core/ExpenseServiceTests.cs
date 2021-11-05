using System;
using NUnit.Framework;
using Moq;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Repositories;
using System.Threading.Tasks;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    public class ExpenseServiceTests
    {
        private static readonly List<Expense> Expenses = new List<Expense>
        {
            new Expense(1, 1.1, Category, "SomeDescription", DateTime.Now.Date),
            new Expense(2, 1.1, null, "SomeDescription", DateTime.Now.Date.AddDays(1)),
            new Expense(3, 1.1, Category, null),
            new Expense(4, 1.1, null, null, DateTime.Now.Date.AddDays(-1)),
        };

        private static readonly Category Category = new Category(1,"Category123", null);

        [Test]
        public async Task Get_ReturnsAllExpenses()
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            expenseRepository.Setup(x => x.Expenses(null)).ReturnsAsync(Expenses);
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);
            
            // Act
            var expenses = await expenseService.Get();

            // Assert
            Assert.IsNotNull(expenses);
            Assert.AreEqual(Expenses.Count(), expenses.Count());
            expenseRepository.Verify(e => e.Expenses(null), Times.Once);
            expenseRepository.VerifyNoOtherCalls();
            categoryRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Get_ReturnsEmptyCollectionIfNoExpenses()
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            expenseRepository.Setup(x => x.Expenses(null)).ReturnsAsync((IEnumerable<Expense>)null);
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);
            
            // Act
            var expenses = await expenseService.Get();

            // Assert
            Assert.IsNotNull(expenses);
            Assert.AreEqual(0, expenses.Count());
            expenseRepository.Verify(e => e.Expenses(null), Times.Once);
            expenseRepository.VerifyNoOtherCalls();
            categoryRepository.VerifyNoOtherCalls();
        }

        [TestCase(10, 0, true)]
        [TestCase(15, 5, false)]
        public async Task GetAll_ReturnsPagedExpenses(int limit, int offset, bool latestFirst)
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            expenseRepository.Setup(x => x.Expenses(null, limit, offset, latestFirst)).ReturnsAsync((IEnumerable<Expense>)Expenses);
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

            // Act
            var expenses = await expenseService.GetAll(null, limit, offset, latestFirst);

            // Assert
            Assert.IsNotNull(expenses);
            Assert.AreEqual(Expenses.Count(), expenses.Count());
            expenseRepository.Verify(e => e.Expenses(null, limit, offset, latestFirst), Times.Once);
            expenseRepository.VerifyNoOtherCalls();
            categoryRepository.VerifyNoOtherCalls();
        }

        [TestCase(10, 0, true)]
        public async Task GetAll_ReturnsEmptyCollectionIfNoExpense(int limit, int offset, bool latestFirst)
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            expenseRepository.Setup(x => x.Expenses(null, limit, offset, latestFirst)).ReturnsAsync((IEnumerable<Expense>)null);
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

            // Act
            var expenses = await expenseService.GetAll(null, limit, offset, latestFirst);

            // Assert
            Assert.IsNotNull(expenses);
            Assert.AreEqual(0, expenses.Count());
            expenseRepository.Verify(e => e.Expenses(null, limit, offset, latestFirst), Times.Once);
            expenseRepository.VerifyNoOtherCalls();
            categoryRepository.VerifyNoOtherCalls();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(1000)]
        public async Task Get_ReturnsExpenseWithParticularId(int id)
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            expenseRepository.Setup(x => x.Get(id)).ReturnsAsync(Expenses.SingleOrDefault(e => e.Id == id));
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

            // Act
            var expense = await expenseService.Get(id);

            // Assert
            Assert.AreEqual(expense, Expenses.SingleOrDefault(e => e.Id == id));
            expenseRepository.Verify(e => e.Get(id), Times.Once);
            expenseRepository.VerifyNoOtherCalls();
            categoryRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Get_ReturnsNullIfCategoryDoesNotExists()
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            categoryRepository.Setup(x => x.Get("NonCategory")).ReturnsAsync((Category)null);
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

            // Act
            var expense = await expenseService.Get("NonCategory");

            // Assert
            Assert.IsNull(expense);
            categoryRepository.Verify(x => x.Get("NonCategory"), Times.Once);
            expenseRepository.VerifyNoOtherCalls();
            categoryRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Get_ReturnsExpensesOfGivenCategory()
        {
            // Arrange
            Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            categoryRepository.Setup(x => x.Get(Category.Name)).ReturnsAsync(Category);
            expenseRepository
                .Setup(x => x.Expenses(It.IsAny<Func<Expense, bool>>()))
                .ReturnsAsync(Expenses.Where(e => e.Category == Category));
            IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

            // Act
            var expenses = await expenseService.Get(Category.Name);

            // Assert
            Assert.IsNotNull(expenses);
            Assert.AreEqual(Expenses.Where(e => e.Category == Category).Count(), expenses.Count());
            // expenseRepository.Verify(x => x.Expenses(e => e.Category != null && e.Category.Name.Equals("Category123", StringComparison.OrdinalIgnoreCase)), Times.Once);
            // expenseRepository.VerifyNoOtherCalls();
            categoryRepository.Verify(x => x.Get(Category.Name), Times.Once);
            categoryRepository.VerifyNoOtherCalls();
        }
    }
}