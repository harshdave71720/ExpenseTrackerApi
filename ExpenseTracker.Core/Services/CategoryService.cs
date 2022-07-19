using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ILogger<CategoryService> logger,ICategoryRepository categoryRepository)
        {
            Guard.AgainstDependencyNull(categoryRepository);
            Guard.AgainstDependencyNull(logger);

            _logger = logger;
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

            if (await _categoryRepository.Exists(category.User, category.Name))
                throw new BadRequestException(ErrorMessages.CategoryAlreadyExists(category.Name));

            var result = await _categoryRepository.Add(category);
            return result;
        }

        public async Task<Category> Delete(User user, string categoryName)
        {
            Guard.AgainstNull(user, nameof(user));
            Guard.AgainstNullOrWhiteSpace(categoryName, nameof(categoryName));

            var category = await _categoryRepository.Get(user, categoryName);
            if (category == null)
                throw new NotFoundException(ErrorMessages.CategoryNotFound(categoryName));
            
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
                throw new NotFoundException(ErrorMessages.CategoryNotFound(category.Name));
            }

            var result = await _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return result;
        }

        public async Task<string[]> GetNames(User user)
        {
            Guard.AgainstNull(user, nameof(user));
            var categories = await _categoryRepository.GetNames(user);
            _logger.LogInformation("Log from service layer : {names}", categories);
            return await _categoryRepository.GetNames(user);
        }
    }
}