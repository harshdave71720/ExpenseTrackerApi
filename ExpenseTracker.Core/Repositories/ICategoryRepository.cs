using ExpenseTracker.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> Add(Category category);

        void Delete(Category category);

        Task<IEnumerable<Category>> Categories(User user);

        Task<Category> Get(User user, string name);

        Task<Category> Update(Category category);

        Task SaveChangesAsync();

        Task<Category> Get(User user, int id);

        Task<IEnumerable<Category>> Get(User user, IEnumerable<string> categories);

        Task<bool> Exists(User user, int id);

        Task<bool> Exists(User user, string name);
    }
}