using ExpenseTracker.Core.Entities;
using System.Collections.Generic;

namespace ExpenseTracker.Core.Repositories
{
    public interface ICategoryRepository
    {
        Category Add(Category c);

        Category Delete(string name);

        IEnumerable<Category> Categories();

        Category Get(string name);
    }
}