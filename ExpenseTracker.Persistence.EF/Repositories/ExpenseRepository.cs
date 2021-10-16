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
            var ex = _mapper.Map<Expense, ExpenseEntity>(expense);
            
            await _context.Expenses.AddAsync(ex);
            await _context.SaveChangesAsync();
            
            var result = _mapper.Map<Expense>(ex);
            result.Category = expense.Category;
            return result;
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
                    .Include(e => e.Category)
                    .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                    .ToListAsync();
            
            return  _context.Expenses
                    .Include(e => e.Category)
                    .ProjectTo<Expense>(_mapper.ConfigurationProvider)
                    .Where(filter)
                    .ToList();
        }


        public async Task<IEnumerable<Expense>> Expenses(Func<Expense, bool> filter, int limit, int offset, bool latestFirst)
        {
            // var result = _context.Expenses
            //                 .Include(e => e.Category);
            // if(filter != null) result = result.Where(filter).AsQueryable();
            if(latestFirst)
                return await _context.Expenses
                                .Include(e => e.Category)
                                .OrderByDescending(e => e.Date)
                                .Skip(offset)
                                .Take(limit)
                                .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                                .ToListAsync();
            else
                return await _context.Expenses
                                .Include(e => e.Category)
                                .Skip(offset)
                                .Take(limit)
                                .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                                .ToListAsync();
        }

        public async Task<Expense> Get(int id)
        {
            return _mapper.Map<ExpenseEntity, Expense>(await _context.Expenses.Include(e => e.Category).SingleOrDefaultAsync(e => e.Id == id));
        }

        public async Task<Expense> Update(Expense expense)
        {
            var expenseToUpdate = await _context.Expenses.SingleOrDefaultAsync(e => e.Id == expense.Id);

            if(expenseToUpdate == null)
                return null;

            expenseToUpdate.Amount = expense.Amount;
            expenseToUpdate.Description = expense.Description;
            expenseToUpdate.Date = expense.Date;
            
            if(expense.Category != null)
                expenseToUpdate.CategoryId = expense.Category.Id;

            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCount()
        {
            // return Task.FromResult<int>(_context.Expenses.Count());
            return await _context.Expenses.CountAsync();
        }
    }
}