using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Helpers.Templates;
using System.IO;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Core.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            Guard.AgainstDependencyNull(expenseRepository);
            Guard.AgainstDependencyNull(categoryRepository);
            Guard.AgainstDependencyNull(userRepository);

            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Expense>> Get(User user)
        {
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.Expenses(user) ?? new List<Expense>();
        }

        public async Task<IEnumerable<Expense>> GetAll(User user, Func<Expense, bool> filter, int limit, int offset, bool latestFirst = true)
        {
            Guard.AgainstNull(user, nameof(user));
            Guard.AgainstZeroOrNegative(limit, nameof(limit));
            Guard.AgainstNegative(offset, nameof(offset));
            return await _expenseRepository.Expenses(user, filter, limit, offset, latestFirst) ?? new List<Expense>();
        }

        public async Task<Expense> Get(User user, int expenseId)
        {
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.Get(user, expenseId);
        }

        public async Task<IEnumerable<Expense>> Get(User user, string categoryName)
        {
            Guard.AgainstNull(user, nameof(user));
            Guard.AgainstNullOrWhiteSpace(categoryName, nameof(categoryName));
            
            var category = await _categoryRepository.Get(user, categoryName);

            if(category == null)
                throw new NotFoundException(ErrorMessages.CategoryNotFound(categoryName));

            return await _expenseRepository.Expenses(user, e => e.Category != null && e.Category.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
                ?? new List<Expense>();
        }

        public async Task<IEnumerable<Expense>> Get(User user, Func<Expense, bool> filter)
        {
            Guard.AgainstNull(user, nameof(user));
            Guard.AgainstNull(filter, nameof(filter));

            return await _expenseRepository.Expenses(user, filter) ?? new List<Expense>();
        }

        public async Task<Expense> Add(Expense expense)
        {
            Guard.AgainstNull(expense, nameof(expense));
            Guard.AgainstNull(expense.User, nameof(User));

            expense = await _expenseRepository.Add(expense);
            return expense;
        }

        public async Task<Expense> Delete(User user, int expenseId)
        {
            Guard.AgainstNull(user, nameof(user));

            var expense = await _expenseRepository.Get(user, expenseId);
            if (expense == null)
                throw new NotFoundException(ErrorMessages.ExpenseNotFound(expenseId));

            _expenseRepository.Delete(expense);
            await _expenseRepository.SaveChangesAsync();

            return expense;
        }

        public async Task<Expense> Update(Expense expense)
        {
            Guard.AgainstNull(expense, nameof(expense));
            Guard.AgainstNull(expense.User, nameof(User));

            if (!await _expenseRepository.Exists(expense.User, expense.Id))
            {
                throw new NotFoundException(ErrorMessages.ExpenseNotFound(expense.Id));
            }
            
            expense = await _expenseRepository.Update(expense);
            await _expenseRepository.SaveChangesAsync();

            return expense;
        }

        public async Task<int> GetExpenseCount(User user)
        {
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.GetCount(user);
        }

        public async Task Add(User user, IEnumerable<KeyValuePair<Expense, string>> expenseWithCategories) 
        {
            Guard.AgainstNull(user, nameof(user));
            var categoryNames = expenseWithCategories.Where(x => x.Value != null).Select(x => x.Value).Distinct().ToList();
            var categories = await _categoryRepository.Get(user, categoryNames);
            var expenses = new List<Expense>();
            var nonExistingCategories = categoryNames.Where(c => !categories.Any(ca => ca.Name.Equals(c, StringComparison.Ordinal)));

            if (nonExistingCategories.Count() > 0)
            {
                throw new ValidationException(nonExistingCategories.Select(c => $"The category : {c} does not exists"));
            }

            foreach (var expenseWithCategory in expenseWithCategories)
            {
                var expense = expenseWithCategory.Key;
                Guard.AgainstNull(expense.User, nameof(User));
                var categoryName = expenseWithCategory.Value;
                expense.Category = categories.SingleOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                expenses.Add(expense);
            }

            await _expenseRepository.Add(expenses);
        }

        private async Task<User> GetUser(string email)
        {
            var user = await this._userRepository.GetUser(email);
            return user;
        }
    }
}