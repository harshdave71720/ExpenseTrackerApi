using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> Get(User user);

        Task<Category> Get(User user, string categoryName);

        Task<Category> Update(Category category);

        Task<Category> Delete(User user, string categoryName);

        Task<Category> Add(Category category);
    }
}