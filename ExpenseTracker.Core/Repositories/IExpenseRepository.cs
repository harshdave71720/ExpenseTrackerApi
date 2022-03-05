using System;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense> Add(Expense expense);

        Task Add(IEnumerable<Expense> expenses);

        void Delete(Expense expense);

        Task<IEnumerable<Expense>> Expenses(User user);

        Task<IEnumerable<Expense>> Expenses(User user, Func<Expense, bool> filter);

        Task<IEnumerable<Expense>> Expenses(User user, Func<Expense, bool> filter, int limit, int offset, bool oldestFirst);

        Task<Expense> Get(User user, int expenseId);

        Task<Expense> Update(Expense expense);

        Task SaveChangesAsync();

        Task<bool> Exists(User user, int expenseId);

        Task<int> GetCount(User user);
    }
}