using System;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense> Add(Expense expense);

        Task<Expense> Delete(int id);

        Task<IEnumerable<Expense>> Expenses(Predicate<Expense> filter);

        Task<Expense> Get(int id);

        public Task<Expense> Update(Expense expense);

        Task SaveChangesAsync();
    }
}