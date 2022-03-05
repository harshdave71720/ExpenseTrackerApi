using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.IO;

namespace ExpenseTracker.Core.Services
{
    public interface IExpenseService
    {
        Task<Expense> Add(string userEmail, Expense expense, string categoryName);

        Task<IEnumerable<Expense>> Get(string userEmail);

        Task<Expense> Get(string userEmail, int expenseId);

        Task<int> GetExpenseCount(string userEmail);

        Task<IEnumerable<Expense>> Get(string userEmail, string category);

        Task<IEnumerable<Expense>> Get(string userEmail, Func<Expense, bool> filter);

        Task<Expense> Delete(string userEmail, int expenseId);

        Task<Expense> Update(string userEmail, Expense expense, string categoryName = null);

        Task<IEnumerable<Expense>> GetAll(string userEmail, Func<Expense, bool> filter, int limit, int offset, bool oldestFirst = false);

        Task<IEnumerable<string>> Add(string userEmail, IEnumerable<KeyValuePair<Expense, string>> expenseWithCategories);
    }
}