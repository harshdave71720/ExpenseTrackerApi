using ExpenseTracker.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> Add(Category category);

        Task<Category> Delete(string name);

        Task<IEnumerable<Category>> Categories();

        Task<Category> Get(string name);

        Task<Category> Update(Category category);
    }
}