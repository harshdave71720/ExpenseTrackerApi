using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> Get();

        Task<Category> Get(string name);

        Task<Category> Update(Category category);

        Task<Category> Delete(string name);
    }
}