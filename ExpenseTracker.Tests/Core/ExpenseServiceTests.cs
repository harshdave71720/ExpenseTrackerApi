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
        private Mock<IUserRepository> NewUserRepositoryMock => new Mock<IUserRepository>();
        [Test]
        public void Instanciation_OnAnyRepositoryNull_ThrowsException()
        {
            // Assert and Act
            Assert.Throws<ArgumentNullException>(
                () => new ExpenseService(null, NewCategoryRepositoryMock.Object, NewUserRepositoryMock.Object)
            );
            Assert.Throws<ArgumentNullException>(
                () => new ExpenseService(NewExpenseRepositoryMock.Object, null, NewUserRepositoryMock.Object)
            );
            Assert.Throws<ArgumentNullException>(
                () => new ExpenseService(NewExpenseRepositoryMock.Object, NewCategoryRepositoryMock.Object, null)
            );
        }

        [Test]
        public void Get_OnEmailNullOrWhiteSpace_ThrowException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                NewUserRepositoryMock.Object);
            // Act
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("     "));

            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(null, 1));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(String.Empty, 1));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("     ", 1));

            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(null, "CategoryName"));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(String.Empty, "CategoryName"));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("     ", "CategoryName"));

            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(null, e => e.Id == 1));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(String.Empty, e => e.Id == 1));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("     ", e => e.Id == 1));
        }

        [Test]
        public void Get_OnUserNotExists_ThrowsException()
        {
            // Arrange
            var userRepoMock = NewUserRepositoryMock;
            userRepoMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                userRepoMock.Object);
            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Get("NotAUser"));
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Get("NotAUser", 1));
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Get("NotAUser", "Category1"));
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Get("NotAUser", filter: null));
        }

        [Test]
        public async Task Get_OnUserExistsAndExpensesPresent_ReturnsExpenses()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(It.IsAny<User>())).ReturnsAsync(_expenses);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                SetUpDefualtUserRepository().Object);
            // Act
            var expenses = await sut.Get("User@abc.com");

            // Assert
            Assert.AreEqual(_expenses, expenses);
        }

        [Test]
        public async Task Get_OnUserExistsAndExpensesNotPresent_ReturnsExptyCollection()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(It.IsAny<User>())).ReturnsAsync((IEnumerable<Expense>)null);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                SetUpDefualtUserRepository().Object);
            // Act
            var expenses = await sut.Get("User@abc.com");

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
                                                                NewCategoryRepositoryMock.Object,
                                                                SetUpDefualtUserRepository().Object);
            // Act
            var expense = await sut.Get("User@abc.com", 1);

            // Assert
            Assert.AreEqual(_expense, expense);
        }

        [Test]
        public void Get_OnCategoryNameNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                        NewCategoryRepositoryMock.Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("userEmail", category: null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("userEmail", category: String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get("userEmail", category: ""));
        }

        [Test]
        public async Task Get_OnCategoryNotExists_ReturnsNull()
        {
            // Arrange
            var categoryRepoMock = NewCategoryRepositoryMock;
            categoryRepoMock.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync((Category)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                        categoryRepoMock.Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            var expenses = await sut.Get("User@abc.com", "Category1");

            // Assert
            Assert.IsNull(expenses);
        }

        [Test]
        public async Task Get_OnCategoryExistsAndExpensesPresent_ReturnsExpenses()
        {
            // Arrange
            var expenseRepoMock = NewExpenseRepositoryMock;
            expenseRepoMock.Setup(x => x.Expenses(It.IsAny<User>(), It.IsAny<Func<Expense, bool>>())).ReturnsAsync(_expenses);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                        SetUpDefualtCategoryRepository().Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            var expenses = await sut.Get("User@abc.com", "Category1");

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
                                                        SetUpDefualtCategoryRepository().Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            var expenses = await sut.Get("User@abc.com", "Category1");

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
                                                        SetUpDefualtCategoryRepository().Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            var expenses = await sut.Get("User@abc.com", e => e.Id == 1);

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
                                                        SetUpDefualtCategoryRepository().Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            var expenses = await sut.Get("User@abc.com", e => e.Id == 1);

            // Assert
            Assert.IsEmpty(expenses);
        }

        [Test]
        public void Add_OnEmailNullOrWhiteSpace_ThrowsException()
        {
            // Arrage
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object,
                                                       NewUserRepositoryMock.Object);

            // Act
            Assert.ThrowsAsync<ArgumentException>(() => sut.Add(null, _expense, categoryName: null));
        }

        [Test]
        public void Add_OnExpenseNull_ThrowsException()
        {
            // Arrage
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object,
                                                       NewUserRepositoryMock.Object);

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add("user@abc.com", null, categoryName: null));
        }

        [Test]
        public void Add_OnUserNotExists_ThrowsException()
        {
            // Arrage
            var userRepoMock = NewUserRepositoryMock;
            userRepoMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object,
                                                       NewUserRepositoryMock.Object);

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add("user@abc.com", null, categoryName: null));
        }

        [Test]
        public async Task Add_OnGivenCategoryNotExists_ReturnsNull()
        {
            // Arrage
            var categoryRepoMock = NewCategoryRepositoryMock;
            categoryRepoMock.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync((Category)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       categoryRepoMock.Object,
                                                       SetUpDefualtUserRepository().Object);

            // Act
            var addedExpense = await sut.Add("user@abc.com", _expense, categoryName: null);

            // Assert
            Assert.IsNull(addedExpense);
        }

        [Test]
        public async Task Add_ProvidedWithUserAndCategory_AddAndReturnsExpense()
        {
            // Arrage
            var expenseRepoMock = NewExpenseRepositoryMock;
            var expenseToAdd = new Expense(amount: 195.678);
            expenseRepoMock.Setup(x => x.Add(It.IsAny<Expense>())).ReturnsAsync(expenseToAdd);
            var categoryRepoMock = SetUpDefualtCategoryRepository();
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                       categoryRepoMock.Object,
                                                       SetUpDefualtUserRepository().Object);

            // Act
            var addedExpense = await sut.Add("user@abc.com", expenseToAdd, "Category1");

            // Assert
            Assert.AreEqual(expenseToAdd, addedExpense);
            expenseRepoMock.Verify(x => x.Add(expenseToAdd), Times.Once);
            categoryRepoMock.Verify(x => x.Get(_user, "Category1"), Times.Once);
        }

        [Test]
        public async Task Add_ProvidedWithUserAndNoCategory_AddAndReturnsExpense()
        {
            // Arrage
            var expenseRepoMock = NewExpenseRepositoryMock;
            var expenseToAdd = new Expense(amount: 195.678);
            expenseRepoMock.Setup(x => x.Add(It.IsAny<Expense>())).ReturnsAsync(expenseToAdd);
            IExpenseService sut = new ExpenseService(expenseRepoMock.Object,
                                                       NewCategoryRepositoryMock.Object,
                                                       SetUpDefualtUserRepository().Object);
            // Act
            var addedExpense = await sut.Add("user@abc.com", expenseToAdd, null);

            // Assert
            Assert.AreEqual(expenseToAdd, addedExpense);
            expenseRepoMock.Verify(x => x.Add(expenseToAdd), Times.Once);
        }

        [Test]
        public void Delete_OnEmailNullOrWhitespace_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object,
                                                       NewUserRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Delete(null, 1));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Delete(String.Empty, 1));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Delete("   ", 1));
        }

        [Test]
        public void Delete_OnUserNotExists_ThrowsException()
        {
            // Arrange
            var userRepoMock = NewUserRepositoryMock;
            userRepoMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                       NewCategoryRepositoryMock.Object,
                                                       userRepoMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Delete("NonExistingUser", 1));
        }

        [Test]
        public async Task Delete_OnExpenseNotExist_ReturnsNull()
        {
            // Arrange
            var expenseRepository = new Mock<IExpenseRepository>();
            expenseRepository.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync((Expense)null);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                        NewCategoryRepositoryMock.Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            Expense deletedExpense = await sut.Delete("abc@xyz.com", 1);

            // Assert
            Assert.IsNull(deletedExpense);
        }

        [Test]
        public async Task Delete_OnExpenseExist_DeletesAndReturnsExpense()
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(_expense);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                SetUpDefualtUserRepository().Object);

            // Act
            Expense deletedExpense = await sut.Delete("Abc@xyz.com", 1);

            // Assert
            Assert.AreEqual(_expense, deletedExpense);
            expenseRepository.Verify(x => x.Delete(_expense), Times.Once);
            expenseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void Update_OnEmailNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                NewUserRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Update(null, _expense, "Category1"));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Update(String.Empty, _expense, "Category1"));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Update("  ", _expense, "Category1"));
        }

        [Test]
        public void Update_OnExpenseNull_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                NewUserRepositoryMock.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Update("abc@xyz.com", null, "Category1"));
        }

        [Test]
        public void Update_OnUserNotExists_ThrowsException()
        {
            // Arrange
            var userRepository = NewUserRepositoryMock;
            userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                userRepository.Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Update("abc@xyz.com", _expense, "Category1"));
        }

        [Test]
        public async Task Update_ExpenseNotPresent_ReturnsNull()
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.Exists(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(false);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                SetUpDefualtUserRepository().Object);

            // Assert
            Expense updatedExpense = await sut.Update("abc@xyz.com", _expense, "Category1");
        }

        [Test]
        public async Task Update_OnCategoryProvidedAndNotExists_ReturnsNull()
        {
            // Arrange
            var categoryRepository = NewCategoryRepositoryMock;
            categoryRepository.Setup(x => x.Get(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync((Category)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                categoryRepository.Object,
                                                                SetUpDefualtUserRepository().Object);
            // Act
            Expense updatedExpense = await sut.Update("abc@xyz.com", _expense);

            // Assert
            Assert.IsNull(updatedExpense);
        }

        [Test]
        public async Task Update_OnExpenseValid_UpdatesAndReturnsExpense()
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.Exists(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(true);
            expenseRepository.Setup(x => x.Update(It.IsAny<Expense>())).ReturnsAsync(_expense);
            IExpenseService sut = new ExpenseService(expenseRepository.Object,
                                                                SetUpDefualtCategoryRepository().Object,
                                                                SetUpDefualtUserRepository().Object);
            // Act
            Expense updatedExpense = await sut.Update("abc@xyz.com", _expense);

            // Assert
            Assert.AreEqual(_expense, updatedExpense);
            expenseRepository.Verify(x => x.Update(_expense), Times.Once);
            expenseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void GetExpenseCount_OnEmailNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                NewUserRepositoryMock.Object);
            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.GetExpenseCount(null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.GetExpenseCount(String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.GetExpenseCount("   "));
        }

        [Test]
        public void GetExpenseCount_OnUserNotExists_ThrowsException()
        {
            // Arrange
            var userRepository = NewUserRepositoryMock;
            userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null);
            IExpenseService sut = new ExpenseService(NewExpenseRepositoryMock.Object,
                                                                NewCategoryRepositoryMock.Object,
                                                                NewUserRepositoryMock.Object);
            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetExpenseCount("abc@xyz.com"));
        }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(1000)]
        public void GetExpenseCount_OnUserExists_ReturnsCountOfExpenses(int expectedCount)
        {
            // Arrange
            var expenseRepository = NewExpenseRepositoryMock;
            expenseRepository.Setup(x => x.GetCount(It.IsAny<User>())).ReturnsAsync(expectedCount);
            IExpenseService sut = new ExpenseService(   expenseRepository.Object, 
                                                        NewCategoryRepositoryMock.Object,
                                                        SetUpDefualtUserRepository().Object);

            // Act
            var expenseCount = sut.GetExpenseCount("abc@xyz.com");

            // Assert
            Assert.AreEqual(expectedCount, expectedCount);
        }

        private Mock<IUserRepository> SetUpDefualtUserRepository()
        {
            var userRepoMock = NewUserRepositoryMock;
            userRepoMock.Setup(m => m.GetUser(It.IsAny<string>())).ReturnsAsync(_user);
            return userRepoMock;
        }

        private Mock<ICategoryRepository> SetUpDefualtCategoryRepository()
        {
            var categoryRepoMock = NewCategoryRepositoryMock;
            categoryRepoMock.Setup(m => m.Get(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(_category);
            return categoryRepoMock;
        }

        //private static readonly User user = new User(1, "Demo User", "Demo", "Demo");
        //private readonly List<Expense> Expenses = new List<Expense>
        //{
        //    new Expense(1, 1, user, Category, "SomeDescription", DateTime.Now.Date),
        //    new Expense(2, 1.1, user, null, "SomeDescription", DateTime.Now.Date.AddDays(1)),
        //    new Expense(3, 1.1, user, Category, null),
        //    new Expense(4, 1.1, user, null, null, DateTime.Now.Date.AddDays(-1)),
        //};

        //private static readonly Category Category = new Category(1, "Category123", user: user);

        //[Test]
        //public async Task Get_ReturnsAllExpenses()
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    expenseRepository.Setup(x => x.Expenses(null)).ReturnsAsync(Expenses);
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expenses = await expenseService.Get();

        //    // Assert
        //    Assert.IsNotNull(expenses);
        //    Assert.AreEqual(Expenses.Count(), expenses.Count());
        //    expenseRepository.Verify(e => e.Expenses(null), Times.Once);
        //    expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.VerifyNoOtherCalls();
        //}

        //[Test]
        //public async Task Get_ReturnsEmptyCollectionIfNoExpenses()
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    expenseRepository.Setup(x => x.Expenses(null)).ReturnsAsync((IEnumerable<Expense>)null);
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expenses = await expenseService.Get();

        //    // Assert
        //    Assert.IsNotNull(expenses);
        //    Assert.AreEqual(0, expenses.Count());
        //    expenseRepository.Verify(e => e.Expenses(null), Times.Once);
        //    expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.VerifyNoOtherCalls();
        //}

        //[TestCase(10, 0, true)]
        //[TestCase(15, 5, false)]
        //public async Task GetAll_ReturnsPagedExpenses(int limit, int offset, bool latestFirst)
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    expenseRepository.Setup(x => x.Expenses(null, limit, offset, latestFirst)).ReturnsAsync((IEnumerable<Expense>)Expenses);
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expenses = await expenseService.GetAll(null, limit, offset, latestFirst);

        //    // Assert
        //    Assert.IsNotNull(expenses);
        //    Assert.AreEqual(Expenses.Count(), expenses.Count());
        //    expenseRepository.Verify(e => e.Expenses(null, limit, offset, latestFirst), Times.Once);
        //    expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.VerifyNoOtherCalls();
        //}

        //[TestCase(10, 0, true)]
        //public async Task GetAll_ReturnsEmptyCollectionIfNoExpense(int limit, int offset, bool latestFirst)
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    expenseRepository.Setup(x => x.Expenses(null, limit, offset, latestFirst)).ReturnsAsync((IEnumerable<Expense>)null);
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expenses = await expenseService.GetAll(null, limit, offset, latestFirst);

        //    // Assert
        //    Assert.IsNotNull(expenses);
        //    Assert.AreEqual(0, expenses.Count());
        //    expenseRepository.Verify(e => e.Expenses(null, limit, offset, latestFirst), Times.Once);
        //    expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.VerifyNoOtherCalls();
        //}

        //[TestCase(1)]
        //[TestCase(2)]
        //[TestCase(3)]
        //[TestCase(1000)]
        //public async Task Get_ReturnsExpenseWithParticularId(int id)
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    expenseRepository.Setup(x => x.Get(id)).ReturnsAsync(Expenses.SingleOrDefault(e => e.Id == id));
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expense = await expenseService.Get(id);

        //    // Assert
        //    Assert.AreEqual(expense, Expenses.SingleOrDefault(e => e.Id == id));
        //    expenseRepository.Verify(e => e.Get(id), Times.Once);
        //    expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.VerifyNoOtherCalls();
        //}

        //[Test]
        //public async Task Get_ReturnsNullIfCategoryDoesNotExists()
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    categoryRepository.Setup(x => x.Get(null, "NonCategory")).ReturnsAsync((Category)null);
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expense = await expenseService.Get("NonCategory");

        //    // Assert
        //    Assert.IsNull(expense);
        //    categoryRepository.Verify(x => x.Get(null, "NonCategory"), Times.Once);
        //    expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.VerifyNoOtherCalls();
        //}

        //[Test]
        //public async Task Get_ReturnsExpensesOfGivenCategory()
        //{
        //    // Arrange
        //    Mock<IExpenseRepository> expenseRepository = new Mock<IExpenseRepository>(MockBehavior.Strict);
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
        //    categoryRepository.Setup(x => x.Get(null, Category.Name)).ReturnsAsync(Category);
        //    expenseRepository
        //        .Setup(x => x.Expenses(It.IsAny<Func<Expense, bool>>()))
        //        .ReturnsAsync(Expenses.Where(e => e.Category == Category));
        //    IExpenseService expenseService = new ExpenseService(expenseRepository.Object, categoryRepository.Object);

        //    // Act
        //    var expenses = await expenseService.Get(Category.Name);

        //    // Assert
        //    Assert.IsNotNull(expenses);
        //    Assert.AreEqual(Expenses.Where(e => e.Category == Category).Count(), expenses.Count());
        //    // expenseRepository.Verify(x => x.Expenses(e => e.Category != null && e.Category.Name.Equals("Category123", StringComparison.OrdinalIgnoreCase)), Times.Once);
        //    // expenseRepository.VerifyNoOtherCalls();
        //    categoryRepository.Verify(x => x.Get(null, Category.Name), Times.Once);
        //    categoryRepository.VerifyNoOtherCalls();
        //}
    }
}