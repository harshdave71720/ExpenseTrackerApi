using ExpenseTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;
using AutoMapper;
using ExpenseTracker.Persistence.Entities;
using AutoMapper.QueryableExtensions;

namespace ExpenseTracker.Persistence.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _context;
        private readonly IMapper _mapper;

        public ExpenseRepository(ExpenseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Expense> Add(Expense expense)
        {
            await _context.Expenses.AddAsync(_mapper.Map<Expense, ExpenseEntity>(expense));
            return expense;
        }

        public async Task<Expense> Delete(int id)
        {
            var expense = await _context.Expenses.SingleOrDefaultAsync(e => e.Id == id);
            if(expense != null)
                _context.Expenses.Remove(expense);
            
            return _mapper.Map<ExpenseEntity, Expense>(expense);
        }

        public async Task<IEnumerable<Expense>> Expenses(Func<Expense, bool> filter)
        {
            if(filter == null)
                return await _context.Expenses
                        .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                        .ToListAsync();
            
            return  _context.Expenses
                        .ProjectTo<Expense>(_mapper.ConfigurationProvider)
                        .Where(filter)
                        .ToList();
        }

        public async Task<Expense> Get(int id)
        {
            return _mapper.Map<ExpenseEntity, Expense>(await _context.Expenses.SingleOrDefaultAsync(e => e.Id == id));
        }

        public async Task<Expense> Update(Expense expense)
        {
            var expenseToUpdate = await _context.Expenses.SingleOrDefaultAsync(e => e.Id == expense.Id);

            if(expenseToUpdate == null)
                return null;

            expenseToUpdate.Amount = expense.Amount;
            expenseToUpdate.CategoryId = expense.Category.Id;

            _context.SaveChangesAsync();
            return expense;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}