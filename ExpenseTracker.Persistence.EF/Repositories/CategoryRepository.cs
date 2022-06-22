using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Persistence.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.DbContexts;
using ExpenseTracker.Core.Repositories;

namespace ExpenseTracker.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ExpenseDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(ExpenseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Category> Add(Category category)
        {
            var categoryEntity = _mapper.Map<CategoryEntity>(category);
            await _context.Categories.AddAsync(categoryEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<Category>(categoryEntity);
        }

        public void Delete(Category category)
        {
            var categoryToDelete = new CategoryEntity { Id = category.Id };
            _context.Attach(categoryToDelete);
            _context.Entry(categoryToDelete).State = EntityState.Deleted;
        }


        public async Task<IEnumerable<Category>> Categories(User user)
        {
            var categories = await _context.Categories.AsNoTracking().Where(c => c.UserId == user.Id).ToListAsync();
            return categories.Select(_mapper.Map<Category>);
        }

        public async Task<Category> Get(User user, string name)
        {
            return _mapper.Map<Category>(await _context.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.Name.Equals(name) && c.UserId == user.Id));
        }

        public async Task<Category> Update(Category category)
        {
            var existing = await _context.Categories.SingleOrDefaultAsync(c => c.Id == category.Id && c.UserId == category.User.Id);
            existing.Name = category.Name;
            return _mapper.Map<Category>(existing);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Category> Get(User user, int id)
        {
            return _mapper.Map<Category>(await _context.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id && c.UserId == user.Id));
        }

        public async Task<IEnumerable<Category>> Get(User user, IEnumerable<string> categories)
        {
            return _mapper.Map<IEnumerable<Category>>(await _context.Categories.AsNoTracking().Where(c => categories.Contains(c.Name) && c.UserId == user.Id).ToListAsync());
        }

        public async Task<bool> Exists(User user, int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id && c.UserId == user.Id);
        }

        public async Task<bool> Exists(User user, string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name.Equals(name) && c.UserId == user.Id);
        }

        public async Task<string[]> GetNames(User user)
        {
            return await _context.Categories.AsNoTracking()
                                            .Where(c => c.UserId == user.Id)
                                            .Select(c => c.Name)
                                            .ToArrayAsync();
        }
    }
}