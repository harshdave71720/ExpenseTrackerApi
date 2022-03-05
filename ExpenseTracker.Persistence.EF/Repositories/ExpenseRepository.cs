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
            var expenseToAdd = _mapper.Map<Expense, ExpenseEntity>(expense);
            
            await _context.Expenses.AddAsync(expenseToAdd);
            await this.SaveChangesAsync();

            return _mapper.Map<Expense>(await _context.Expenses.Include(e => e.Category).FirstAsync(e => e.Id == expenseToAdd.Id));
        }

        public async Task Add(IEnumerable<Expense> expenses)
        {
            var entities = _mapper.Map<IEnumerable<ExpenseEntity>>(expenses);
            await _context.Expenses.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public void Delete(Expense expense)
        {
            var expenseToDelete = _mapper.Map<ExpenseEntity>(expense);
            _context.Attach(expenseToDelete);
            _context.Entry(expenseToDelete).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<Expense>> Expenses(User user)
        {
                return await _context.Expenses
                    .Where(e => e.UserId == user.Id)
                    .Include(e => e.Category)
                    .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                    .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> Expenses(User user, Func<Expense, bool> filter)
        {
            return  _context.Expenses
                    .Where(e => e.UserId == user.Id)
                    .Include(e => e.Category)
                    .ProjectTo<Expense>(_mapper.ConfigurationProvider)
                    .Where(filter)
                    .ToList();
        }


        public async Task<IEnumerable<Expense>> Expenses(User user, Func<Expense, bool> filter, int limit, int offset, bool latestFirst)
        {
            // var result = _context.Expenses
            //                 .Include(e => e.Category);
            // if(filter != null) result = result.Where(filter).AsQueryable();
            if(latestFirst)
                return await _context.Expenses
                                .Where(e => e.UserId == user.Id)
                                .Include(e => e.Category)
                                .OrderByDescending(e => e.Date)
                                .Skip(offset)
                                .Take(limit)
                                .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                                .ToListAsync();
            else
                return await _context.Expenses
                                .Where(e => e.UserId == user.Id)
                                .Include(e => e.Category)
                                .Skip(offset)
                                .Take(limit)
                                .Select(e => _mapper.Map<ExpenseEntity, Expense>(e))
                                .ToListAsync();
        }

        public async Task<Expense> Get(User user, int expenseId)
        {
            return _mapper.Map<ExpenseEntity, Expense>(await _context.Expenses.AsNoTracking().Include(e => e.Category).SingleOrDefaultAsync(e => e.Id == expenseId && e.UserId == user.Id));
        }

        public async Task<Expense> Update(Expense expense)
        {
            var expenseToUpdate = _mapper.Map<ExpenseEntity>(expense);
            _context.Attach(expenseToUpdate);
            _context.Entry(expenseToUpdate).State = EntityState.Modified;

            return expense;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCount(User user)
        {
            return await _context.Expenses.CountAsync(e => e.UserId == user.Id);
        }

        public async Task<bool> Exists(User user, int expenseId)
        {
            return await _context.Expenses.AnyAsync(e => e.Id == expenseId && e.UserId == user.Id);
        }
    }
}