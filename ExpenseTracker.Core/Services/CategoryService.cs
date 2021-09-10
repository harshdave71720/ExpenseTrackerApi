using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;

namespace ExpenseTracker.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> Get()
        {
            return await _categoryRepository.Categories();
        }

        public async Task<Category> Get(string name)
        {
            return await _categoryRepository.Get(name);
        }

        public async Task<Category> Add(Category category)
        {
            var result = await _categoryRepository.Add(category);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Category> Delete(string name)
        {
            var result = await _categoryRepository.Delete(name);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Category> Update(Category category)
        {
            var result = await _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }
    }
}