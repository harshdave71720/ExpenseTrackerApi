using System;
using NUnit.Framework;
using Moq;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Repositories;
using System.Threading.Tasks;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    public class ExpenseServiceTests
    {
        private User _user = new User(1, "FirstUser@abc.com", "Firstname", "Lastname");

        private Category _category = new Category(1, "Category1");

        private List<Expense> _expenses = new List<Expense>()
        {
            new Expense(amount : 100),
            new Expense(amount : 20),
            new Expense(amount : 200)
        };
        private Expense _expense = new Expense(amount: 1001.1);
        private Mock<IExpenseRepository> NewExpenseRepositoryMock => new Mock<IExpenseRepository>();
        private Mock<ICategoryRepository> NewCategoryRepositoryMock => new Mock<ICategoryRepository>();
        //private Mock<IUserRepository> NewUserRepositoryMock => new Mock<IUserRepository>();
        [Test]
        public void Instanciation_OnAnyRepositoryNull_ThrowsException()
        {
            // Assert and Act
            Assert.Throws<DependencyNullException>(
                () => new ExpenseService(null, NewCategoryRepositoryMock.Object)
            );
            Assert.Throws<DependencyNullException>(
                () => new ExpenseService(NewExpenseRepositoryMock.Object, null)
            );
        }

        [Test]
        public void Get_UserNull_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object);
            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Get(null));
        }

        [Test]
        public async Task Get_OnUserExistsAndExpensesPresent_ReturnsExpenses()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(_user)).ReturnsAsync(_expenses);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                                NewCategoryRepositoryMock.Object);
            // Act
            var expenses = await sut.Get(_user);

            // Assert
            Assert.AreEqual(_expenses, expenses);
        }

        [Test]
        public async Task Get_OnUserExistsAndExpensesNotPresent_ReturnsEmptyCollection()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(_user)).ReturnsAsync((IEnumerable<Expense>)null);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                                NewCategoryRepositoryMock.Object);
            // Act
            var expenses = await sut.Get(_user);

            // Assert
            Assert.IsEmpty(expenses);
        }

        [Test]
        public async Task Get_OnUserExistsAndExpenseWithIdPresent_ReturnsExpense()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Get(_user, It.IsAny<int>())).ReturnsAsync(_expense);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                                NewCategoryRepositoryMock.Object);
            // Act
            var expense = await sut.Get(_user, 1);

            // Assert
            Assert.AreEqual(_expense, expense);
        }

        [Test]
        public void Get_OnCategoryNameNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                        NewCategoryRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(_user, category: null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(_user, category: String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(_user, category: ""));
        }

        [Test]
        public void Get_OnCategoryNotExists_ThrowsException()
        {
            // Arrange
            var categoryRepoMock = NewCategoryRepositoryMock;
            categoryRepoMock.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync((Category)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                        categoryRepoMock.Object);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => sut.Get(_user, "Category1"));
        }

        [Test]
        public async Task Get_OnCategoryExistsAndExpensesPresent_ReturnsExpenses()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(It.IsAny<User>(), It.IsAny<Func<Expense, bool>>())).ReturnsAsync(_expenses);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                        SetUpDefualtCategoryRepository().Object);

            // Act
            var expenses = await sut.Get(_user, "Category1");

            // Assert
            Assert.AreEqual(_expenses, expenses);
        }

        [Test]
        public async Task Get_OnCategoryExistsAndExpensesNotPresent_ReturnsEmptyCollection()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(It.IsAny<User>(), It.IsAny<Func<Expense, bool>>())).ReturnsAsync((IEnumerable<Expense>)null);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                        SetUpDefualtCategoryRepository().Object);

            // Act
            var expenses = await sut.Get(_user, "Category1");

            // Assert
            Assert.IsEmpty(expenses);
        }

        [Test]
        public async Task Get_OnExpenseWithFilterExists_ReturnsExpenses()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(It.IsAny<User>(), It.IsAny<Func<Expense, bool>>())).ReturnsAsync(_expenses);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                        SetUpDefualtCategoryRepository().Object);

            // Act
            var expenses = await sut.Get(_user, e => e.Id == 1);

            // Assert
            Assert.AreEqual(_expenses, expenses);
        }

        [Test]
        public async Task Get_OnExpenseWithFilterNotExists_ReturnsEmptyCollections()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock
                .Setup(x => x.Expenses(It.IsAny<User>(), It.IsAny<Func<Expense, bool>>()))
                .ReturnsAsync((IEnumerable<Expense>)null);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                        SetUpDefualtCategoryRepository().Object);

            // Act
            var expenses = await sut.Get(_user, e => e.Id == 1);

            // Assert
            Assert.IsEmpty(expenses);
        }

        [Test]
        public void Add_OnExpenseNull_ThrowsException()
        {
            // Arrage
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object);

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(null));
        }

        [Test]
        public void Add_OnUserNull_ThrowsException()
        {
            // Arrage
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object);

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(_expense));
        }

        [Test]
        public async Task Add_ProvidedWithUserAndCategory_AddAndReturnsExpense()
        {
            // Arrage
            var expenseRepoMock = NewExpenseRepositoryMock;
            var expenseToAdd = new Expense(amount: 195.678, _user);
            expenseRepoMock.Setup(x => x.Add(It.IsAny<Expense>())).ReturnsAsync(expenseToAdd);
            var categoryRepoMock = SetUpDefualtCategoryRepository();
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                       categoryRepoMock.Object);

            // Act
            var addedExpense = await sut.Add(expenseToAdd);

            // Assert
            Assert.AreEqual(expenseToAdd, addedExpense);
            expenseRepoMock.Verify(x => x.Add(expenseToAdd), Times.Once);
        }

        [Test]
        public async Task Add_ProvidedWithUserAndNoCategory_AddAndReturnsExpense()
        {
            // Arrage
            var expenseToAdd = new Expense(amount: 195.678, _user);
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Add(It.IsAny<Expense>())).ReturnsAsync(expenseToAdd);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                       NewCategoryRepositoryMock.Object);
            // Act
            var addedExpense = await sut.Add(expenseToAdd);

            // Assert
            Assert.AreEqual(expenseToAdd, addedExpense);
            expenseRepoMock.Verify(x => x.Add(expenseToAdd), Times.Once);
        }

        [Test]
        public void Delete_OnUserNull_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Delete(null, 1));
        }

        [Test]
        public void Delete_OnExpenseNotExist_ThrowsException()
        {
            // Arrange
            var expenseRepository = new Mock<IExpenseRepository>();
            expenseRepository.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync((Expense)null);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                        NewCategoryRepositoryMock.Object);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => sut.Delete(_user, 1));
        }

        [Test]
        public async Task Delete_OnExpenseExist_DeletesAndReturnsExpense()
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(_expense);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                                NewCategoryRepositoryMock.Object);

            // Act
            Expense deletedExpense = await sut.Delete(_user, 1);

            // Assert
            Assert.AreEqual(_expense, deletedExpense);
            expenseRepository.Verify(x => x.Delete(_expense), Times.Once);
            expenseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void Update_OnExpenseNull_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Update(null));
        }

        [Test]
        public void Update_OnUserNull_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Update(new Expense(100)));
        }

        [Test]
        public void Update_ExpenseNotPresent_ThrowsException()
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.Exists(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(false);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                                NewCategoryRepositoryMock.Object);

            // Assert
            Assert.ThrowsAsync<NotFoundException>(() => sut.Update(new Expense(1, _user)));
        }

        [Test]
        public async Task Update_OnExpenseValid_UpdatesAndReturnsExpense()
        {
            // Arrange
            var expense = new Expense(112, _user);
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.Exists(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(true);
            expenseRepository.Setup(x => x.Update(It.IsAny<Expense>())).ReturnsAsync(_expense);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                                SetUpDefualtCategoryRepository().Object);
            // Act
            Expense updatedExpense = await sut.Update(expense);

            // Assert
            Assert.AreEqual(_expense, updatedExpense);
            expenseRepository.Verify(x => x.Update(expense), Times.Once);
            expenseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void GetExpenseCount_OnUser_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object);
            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetExpenseCount(null));
        }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(1000)]
        public void GetExpenseCount_OnUserExists_ReturnsCountOfExpenses(int expectedCount)
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.GetCount(It.IsAny<User>())).ReturnsAsync(expectedCount);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                        NewCategoryRepositoryMock.Object);

            // Act
            var expenseCount = sut.GetExpenseCount(_user);

            // Assert
            Assert.AreEqual(expectedCount, expectedCount);
        }

        private Mock<ICategoryRepository> SetUpDefualtCategoryRepository()
        {
            var categoryRepoMock = NewCategoryRepositoryMock;
            categoryRepoMock.Setup(m => m.Get(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(_category);
            return categoryRepoMock;
        }
    }
}