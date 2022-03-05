using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;

namespace ExpenseTracker.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            Guard.AgainstNull(categoryRepository, nameof(categoryRepository));

            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> Get(User user)
        {
            Guard.AgainstNull(user, nameof(user));
            return await _categoryRepository.Categories(user) ?? new List<Category>();
        }

        public async Task<Category> Get(User user, string categoryName)
        {
            Guard.AgainstNull(user, nameof(user));
            Guard.AgainstNullOrWhiteSpace(categoryName, nameof(categoryName));

            return await _categoryRepository.Get(user, categoryName);
        }

        public async Task<Category> Add(Category category)
        {
            Guard.AgainstNull(category, nameof(category));
            Guard.AgainstNull(category.User, nameof(category.User));

            if (await this._categoryRepository.Exists(category.User, category.Name))
                return null;

            var result = await _categoryRepository.Add(category);
            return result;
        }

        public async Task<Category> Delete(User user, string categoryName)
        {
            Guard.AgainstNull(user, nameof(user));
            Guard.AgainstNullOrWhiteSpace(categoryName, nameof(categoryName));

            var category = await _categoryRepository.Get(user, categoryName);
            if (category == null)
                return null;
            
            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task<Category> Update(Category category)
        {
            Guard.AgainstNull(category, nameof(category));
            Guard.AgainstNull(category.User, nameof(category.User));
            if (!await _categoryRepository.Exists(category.User, category.Id))
            {
                return null;
            }

            var result = await _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }
    }
}