using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.IO;

namespace ExpenseTracker.Core.Services
{
    public interface IExpenseService
    {
        Task<Expense> Add(Expense expense);

        Task<IEnumerable<Expense>> Get(User user);

        Task<Expense> Get(User user, int expenseId);

        Task<int> GetExpenseCount(User user);

        Task<IEnumerable<Expense>> Get(User user, string category);

        Task<IEnumerable<Expense>> Get(User user, Func<Expense, bool> filter);

        Task<Expense> Delete(User user, int expenseId);

        Task<Expense> Update(Expense expense);

        Task<IEnumerable<Expense>> GetAll(User user, Func<Expense, bool> filter, int limit, int offset, bool oldestFirst = false);

        Task Add(User user, IEnumerable<KeyValuePair<Expense, string>> expenseWithCategories);
    }
}