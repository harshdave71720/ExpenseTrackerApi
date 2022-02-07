using System;
using NUnit.Framework;
using AutoMapper;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using Moq;
using System.Threading.Tasks;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private static User user = new User(1, "Demo User", "Demo", "Demo");

        [Test]
        public async Task Update_UpdatesExistingCategory()
        {
            // Arrange
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>();
            var category = new Category(1, "Category1", user);
            categoryRepository.Setup(x => x.Update(category)).ReturnsAsync(new Category(1, "Category1", user));
            categoryRepository.Setup(x => x.Exists(category.Id)).Returns(true);
            var categoryService = new CategoryService(categoryRepository.Object);

            // Act
            var updatedCategory = await categoryService.Update(category);

            // Assert
            Assert.IsNotNull(updatedCategory);
            AssertCategory(category, updatedCategory);
            // categoryRepository.Verify(x => x.Update(category), Times.Once);
            // categoryRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            // categoryRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Update_ReturnsNullIfCategoryNotExists()
        {
            // Arrange
            Mock<ICategoryRepository> categoryRepository = new Mock<ICategoryRepository>();
            var category = new Category(1, "Category1", user);
            categoryRepository.Setup(x => x.Exists(category.Id)).Returns(false);
            var categoryService = new CategoryService(categoryRepository.Object);

            // Act
            var updatedCategory = await categoryService.Update(category);

            // Assert
            Assert.IsNull(updatedCategory);
        }

        private void AssertCategory(Category expected, Category current)
        {
            Assert.AreEqual(expected.Id, current.Id);
            Assert.AreEqual(expected.Name, current.Name);
        }
    }
}