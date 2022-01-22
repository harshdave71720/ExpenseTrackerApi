using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Helpers.Templates;
using System.IO;
using ExpenseTracker.Core.Dtos;

namespace ExpenseTracker.Core.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository)
        {
            Guard.AgainstNull(expenseRepository, nameof(expenseRepository));
            Guard.AgainstNull(categoryRepository, nameof(categoryRepository));

            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
        }

        public async Task<IEnumerable<Expense>> Get()
        {
            return await _expenseRepository.Expenses(null) ?? new List<Expense>();
        }

        public async Task<IEnumerable<Expense>> GetAll(Func<Expense, bool> filter, int limit, int offset, bool latestFirst = true)
        {
            Guard.AgainstZeroOrNegative(limit, nameof(limit));
            Guard.AgainstNegative(offset, nameof(offset));
            return await _expenseRepository.Expenses(filter, limit, offset, latestFirst) ?? new List<Expense>();
        }

        public async Task<Expense> Get(int id)
        {
            Guard.AgainstZeroOrNegative(id, nameof(id));
            return await _expenseRepository.Get(id);
        }

        public async Task<IEnumerable<Expense>> Get(string categoryName)
        {
            Guard.AgainstNullOrWhiteSpace(categoryName, nameof(categoryName));

            var category = await _categoryRepository.Get(categoryName);

            if(category == null)
                return null;

            return await _expenseRepository.Expenses(e => e.Category != null && e.Category.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
                ?? new List<Expense>();
        }

        public async Task<IEnumerable<Expense>> Get(Func<Expense, bool> filter)
        {
            Guard.AgainstNull(filter, nameof(filter));

            var expenses = await _expenseRepository.Expenses(filter) ?? new List<Expense>();
            
            return expenses;
        }

        public async Task<Expense> Add(Expense expense, string categoryName = null)
        {
            Guard.AgainstNull(expense, nameof(expense));

            if (categoryName != null)
            {
                var category = await _categoryRepository.Get(categoryName);
                if (category == null)
                    return null;

                expense.Category = category;
            }

            expense = await _expenseRepository.Add(expense);
            return expense;
        }

        public async Task<Expense> Delete(int id)
        {
            Guard.AgainstZeroOrNegative(id, nameof(id));
            var expense = await _expenseRepository.Delete(id);
            await _expenseRepository.SaveChangesAsync();

            return expense;
        }

        public async Task<Expense> Update(Expense expense, string categoryName = null)
        {
            Guard.AgainstNull(expense, nameof(expense));

            Category category = null;
            if(categoryName != null)
            {
                category = await _categoryRepository.Get(categoryName);
                if(category == null)
                    return null;
            }

            expense.Category = category;
            expense = await _expenseRepository.Update(expense);
            await _expenseRepository.SaveChangesAsync();

            return expense;
        }

        public async Task<int> GetExpenseCount()
        {
            return await _expenseRepository.GetCount();
        }

        public async Task<IEnumerable<string>> Add(IEnumerable<KeyValuePair<Expense, string>> expenseWithCategories) 
        {
            List<string> errors = new List<string>();
            var categoryNames = expenseWithCategories.Select(x => x.Value).Distinct().ToList();
            var categories = await _categoryRepository.Get(categoryNames);

            foreach (var pair in expenseWithCategories)
            {
                var expense = pair.Key;
                var categoryName = pair.Value;
                if (string.IsNullOrWhiteSpace(categoryName))
                    continue;

                var category = categories.SingleOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                if (category == null)
                    errors.Add($"Cannot find the category {pair.Value}");
                else
                    expense.Category = category;
            }

            if (errors.Count() > 0)
                return errors;

            await _expenseRepository.Add(expenseWithCategories.Select(x => x.Key));
            return errors;
        }
    }
}