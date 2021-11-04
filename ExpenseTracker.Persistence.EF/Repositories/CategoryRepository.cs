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
            var exists = _context.Categories.Any(c => c.Name.Equals(category.Name));
            
            if(exists)
                return null;

            var categoryEntity = _mapper.Map<CategoryEntity>(category);
            await _context.Categories.AddAsync(categoryEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<Category>(categoryEntity);
        }

        public async Task<Category> Delete(string name)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name));

            if(category == null)
                return null;
            
            _context.Categories.Remove(category);
            
            return _mapper.Map<Category>(category);
        }


        public async Task<IEnumerable<Category>> Categories()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            return categories.Select(_mapper.Map<Category>);
        }

        public async Task<Category> Get(string name)
        {
            //return _mapper.Map<Category>(await _context.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            return _mapper.Map<Category>(await _context.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.Name.Equals(name)));
        }

        public async Task<Category> Update(Category category)
        {
            var existing = await _context.Categories.SingleOrDefaultAsync(c => c.Id == category.Id);
            if(existing == null)
                return null;
            
            existing.Name = category.Name;
            
            await SaveChangesAsync();
            return _mapper.Map<Category>(await _context.Categories.SingleOrDefaultAsync(c => c.Id == category.Id));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // private async Task<CategoryEntity> GetCategory(string name)
        // {
        //     return await _context.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name));
        // }

        // private async Task<CategoryEntity> GetCategory(int id)
        // {
        //     return await 
        // }
    }
}