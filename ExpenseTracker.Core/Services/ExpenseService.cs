using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Helpers.Templates;
using System.IO;

namespace ExpenseTracker.Core.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            Guard.AgainstNull(expenseRepository, nameof(expenseRepository));
            Guard.AgainstNull(categoryRepository, nameof(categoryRepository));
            Guard.AgainstNull(userRepository, nameof(userRepository));

            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Expense>> Get(string userEmail)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.Expenses(user) ?? new List<Expense>();
        }

        public async Task<IEnumerable<Expense>> GetAll(string userEmail, Func<Expense, bool> filter, int limit, int offset, bool latestFirst = true)
        {
            Guard.AgainstZeroOrNegative(limit, nameof(limit));
            Guard.AgainstNegative(offset, nameof(offset));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.Expenses(user, filter, limit, offset, latestFirst) ?? new List<Expense>();
        }

        public async Task<Expense> Get(string userEmail, int expenseId)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.Get(user, expenseId);
        }

        public async Task<IEnumerable<Expense>> Get(string userEmail, string categoryName)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            Guard.AgainstNullOrWhiteSpace(categoryName, nameof(categoryName));

            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            
            var category = await _categoryRepository.Get(user, categoryName);

            if(category == null)
                return null;

            return await _expenseRepository.Expenses(user, e => e.Category != null && e.Category.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
                ?? new List<Expense>();
        }

        public async Task<IEnumerable<Expense>> Get(string userEmail, Func<Expense, bool> filter)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            Guard.AgainstNull(filter, nameof(filter));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            var expenses = await _expenseRepository.Expenses(user, filter) ?? new List<Expense>();
            
            return expenses;
        }

        public async Task<Expense> Add(string userEmail, Expense expense, string categoryName = null)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            Guard.AgainstNull(expense, nameof(expense));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));

            if (categoryName != null)
            {
                var category = await _categoryRepository.Get(user, categoryName);
                if (category == null)
                    return null;

                expense.Category = category;
            }

            expense.User = user;
            expense = await _expenseRepository.Add(expense);
            return expense;
        }

        public async Task<Expense> Delete(string userEmail, int expenseId)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));

            var expense = await _expenseRepository.Get(user, expenseId);
            if (expense == null)
                return null;

            _expenseRepository.Delete(expense);
            await _expenseRepository.SaveChangesAsync();

            return expense;
        }

        public async Task<Expense> Update(string userEmail, Expense expense, string categoryName = null)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            Guard.AgainstNull(expense, nameof(expense));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            Category category = null;

            if (!await _expenseRepository.Exists(user, expense.Id))
            { 
                return null;
            }
            
            if(categoryName != null)
            {
                category = await _categoryRepository.Get(user, categoryName);
                if(category == null)
                    return null;
            }

            expense.Category = category;
            expense.User = user;
            expense = await _expenseRepository.Update(expense);
            await _expenseRepository.SaveChangesAsync();

            return expense;
        }

        public async Task<int> GetExpenseCount(string userEmail)
        {
            Guard.AgainstNullOrWhiteSpace(userEmail, nameof(userEmail));
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));
            return await _expenseRepository.GetCount(user);
        }

        public async Task<IEnumerable<string>> Add(string userEmail, IEnumerable<KeyValuePair<Expense, string>> expenseWithCategories) 
        {
            User user = await GetUser(userEmail);
            Guard.AgainstNull(user, nameof(user));

            List<string> errors = new List<string>();
            var categoryNames = expenseWithCategories.Select(x => x.Value).Distinct().ToList();
            var categories = await _categoryRepository.Get(null, categoryNames);
            var expenses = new List<Expense>();

            foreach (var pair in expenseWithCategories)
            {
                var expense = pair.Key;
                var categoryName = pair.Value;
                if (!string.IsNullOrWhiteSpace(categoryName))
                {
                    var category = categories.SingleOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                    if (category == null)
                        errors.Add($"Cannot find the category {pair.Value}");
                    else
                        expense.Category = category;
                }

                expense.User = user;
                expenses.Add(expense);
            }

            if (errors.Count() > 0)
                return errors;

            await _expenseRepository.Add(expenses);
            return errors;
        }

        private async Task<User> GetUser(string email)
        {
            var user = await this._userRepository.GetUser(email);
            return user;
        }
    }
}