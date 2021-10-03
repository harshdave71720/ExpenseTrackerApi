using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Helpers;

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

            expense.Category = await _categoryRepository.Get(categoryName);
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
    }
}