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

        public async Task<IEnumerable<Category>> Get()
        {
            return await _categoryRepository.Categories();
        }

        public async Task<Category> Get(string name)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));

            return await _categoryRepository.Get(name);
        }

        public async Task<Category> Add(Category category)
        {
            Guard.AgainstNull(category, nameof(category));

            var result = await _categoryRepository.Add(category);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Category> Delete(string name)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));

            var result = await _categoryRepository.Delete(name);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Category> Update(Category category)
        {
            Guard.AgainstNull(category, nameof(category));
            if(!_categoryRepository.Exists(category.Id))
            {
                return null;
            }

            var result = await _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }
    }
}