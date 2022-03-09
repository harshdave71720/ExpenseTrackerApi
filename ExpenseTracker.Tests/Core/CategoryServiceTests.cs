using System;
using NUnit.Framework;
using AutoMapper;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    public class CategoryServiceTests
    {
        [Test]
        public void Instanciation_OnRepositoryNull_ThrowsException()
        {
            Assert.Throws<DependencyNullException>(() => { new CategoryService(categoryRepository: null); });
        }

        [Test]
        public void Get_OnUserNull_ThrowsException()
        {
            // Arrange 
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>( () => sut.Get(user : null));
            Assert.ThrowsAsync<ArgumentNullException>((() => sut.Get(null, "Category1")));
        }

        [Test]
        public async Task Get_OnNoCategoriesPresent_ReturnsEmptyCollection()
        { 
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Categories(_user)).ReturnsAsync((IEnumerable<Category>)null);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var categories = await sut.Get(_user);

            // Assert
            Assert.IsEmpty(categories);
        }

        [Test]
        public async Task Get_OnCategoriesPresent_ReturnsCategories()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Categories(_user)).ReturnsAsync(_categories);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var categories = await sut.Get(_user);

            // Assert
            Assert.AreEqual(_categories, categories);
        }

        [Test]
        public void Get_OnCategoryNameNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(_user, null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(_user, String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Get(_user, "     "));
        }

        [Test]
        public async Task Get_OnCategoryPresent_ReturnsCategory()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync(_categories[0]);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var category = await sut.Get(_user, "category");

            // Assert
            Assert.AreEqual(_categories[0], category);
        }

        [Test]
        public async Task Get_OnCategoryNotPresent_ReturnsNull()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync((Category)null);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var category = await sut.Get(_user, "category");

            // Assert
            Assert.IsNull(category);
        }

        [Test]
        public void Add_OnCategoryNull_ThrowException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(null) );
        }

        [Test]
        public void Add_OnUserNull_ThrowsException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);
            var categoryToAdd = new Category(100, "Category100", user : null);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(categoryToAdd));
        }

        [Test]
        public async Task Add_OnCategoryAlreadyExists_ReturnsNull()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Exists(_user, It.IsAny<string>())).ReturnsAsync(true);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var category = await sut.Get(_user, "category");

            // Assert
            Assert.IsNull(category);
        }

        [Test]
        public async Task Add_OnNewCategory_SaveAndReturnsAddedCategory()
        {
            // Arrange
            var categoryToAdd = new Category(1000, "CategoryToAdd", user : _user);
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Exists(_user, It.IsAny<string>())).ReturnsAsync(false);
            categoryRepository.Setup(x => x.Add(categoryToAdd)).ReturnsAsync(_categories[0]);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var category = await sut.Add(categoryToAdd);

            // Assert
            Assert.AreEqual(_categories[0], category);
        }

        [Test]
        public void Delete_OnUserNull_ThrowsException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Delete(null, "category1"));
        }

        [Test]
        public void Delete_CategoryNameNullOrWhiteSpace_ThrowsException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.Delete(_user, null));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Delete(_user, String.Empty));
            Assert.ThrowsAsync<ArgumentException>(() => sut.Delete(_user, "   "));
        }

        [Test]
        public void Delete_OnCategoryNotExists_ThrowsException()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync((Category)null);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => sut.Delete(_user, "categoryNotExisting"));
        }

        [Test]
        public async Task Delete_OnCategoryExists_DeletesAndReturnsCategory()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Get(_user, It.IsAny<string>())).ReturnsAsync(_categories[0]);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var category = await sut.Delete(_user, "categoryNotExisting");

            // Assert
            Assert.AreEqual(_categories[0], category);
            categoryRepository.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void Update_OnCategoryNull_ThrowsException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Update(null));
        }
        
        [Test]
        public void Update_OnUserNull_ThrowsException()
        {
            // Arrange
            ICategoryService sut = new CategoryService(new Mock<ICategoryRepository>().Object);
            var categoryToUpdate = new Category(101, "CategoryToUpdate", user : null);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Update(categoryToUpdate));
        }

        [Test]
        public void Update_OnCategoryNotExists_ThrowsException()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Exists(_user, It.IsAny<int>())).ReturnsAsync(false);
            ICategoryService sut = new CategoryService(categoryRepository.Object);
            var categoryToUpdate = new Category(101, "CategoryToUpdate", _user);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => sut.Update(categoryToUpdate));
        }

        [Test]
        public async Task Update_OnCategoryExists_UpdateAndReturnsCategory()
        {
            // Arrange
            var categoryToUpdate = new Category(101, "CategoryToUpdate", _user);
            var categoryRepository = new Mock<ICategoryRepository>();
            categoryRepository.Setup(x => x.Exists(_user, It.IsAny<int>())).ReturnsAsync(true);
            categoryRepository.Setup(x => x.Update(categoryToUpdate)).ReturnsAsync(_categories[0]);
            ICategoryService sut = new CategoryService(categoryRepository.Object);

            // Act
            var updatedCategory = await sut.Update(categoryToUpdate);

            // Assert
            Assert.AreEqual(_categories[0], updatedCategory);
            categoryRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        private static User _user = new User(1, "Demo User", "Demo", "Demo");

        //private Category NewCategory => new Category(100, "Category100");
        private List<Category> _categories = new List<Category>()
        {
            new Category(1, "Category1"),
            new Category(2, "Category2"),
            new Category(3, "Category3")
        };

        //[Test]
        //public async Task Update_UpdatesExistingCategory()
        //{
        //    // Arrange
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>();
        //    var category = new Category(1, "Category1", user);
        //    categoryRepository.Setup(x => x.Update(category)).ReturnsAsync(new Category(1, "Category1", user));
        //    categoryRepository.Setup(x => x.Exists(null, category.Id)).Returns(true);
        //    var categoryService = new CategoryService(categoryRepository.Object);

        //    // Act
        //    var updatedCategory = await categoryService.Update(category);

        //    // Assert
        //    Assert.IsNotNull(updatedCategory);
        //    AssertCategory(category, updatedCategory);
        //    // categoryRepository.Verify(x => x.Update(category), Times.Once);
        //    // categoryRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        //    // categoryRepository.VerifyNoOtherCalls();
        //}

        //[Test]
        //public async Task Update_ReturnsNullIfCategoryNotExists()
        //{
        //    // Arrange
        //    Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>();
        //    var category = new Category(1, "Category1", user);
        //    categoryRepository.Setup(x => x.Exists(null, category.Id)).Returns(false);
        //    var categoryService = new CategoryService(categoryRepository.Object);

        //    // Act
        //    var updatedCategory = await categoryService.Update(category);

        //    // Assert
        //    Assert.IsNull(updatedCategory);
        //}

        //private void AssertCategory(Category expected, Category current)
        //{
        //    Assert.AreEqual(expected.Id, current.Id);
        //    Assert.AreEqual(expected.Name, current.Name);
        //}
    }
}