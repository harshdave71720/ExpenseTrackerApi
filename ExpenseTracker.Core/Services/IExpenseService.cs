using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Services
{
    public interface IExpenseService
    {
        Task<Expense> Add(Expense expense, string categoryName);

        Task<IEnumerable<Expense>> Get();

        Task<Expense> Get(int id);

        Task<IEnumerable<Expense>> Get(string category);

        Task<IEnumerable<Expense>> Get(Func<Expense, bool> filter);

        Task<Expense> Delete(int id);

        Task<Expense> Update(Expense expense, string categoryName);
    }
}