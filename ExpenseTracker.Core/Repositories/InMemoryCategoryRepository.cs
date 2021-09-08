using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Repositories
{
    public class InMemoryCategoryRepository //: ICategoryRepository
    {
        private static List<Category> _categories = new List<Category>
        {
            new Category("Category1", null),
            new Category("Category2", null),
            new Category("Category3", null),
            new Category("Category4", null),
        };

        public Category Add(Category c)
        {
            _categories.Add(c);
            return c;
        }

        public Category Delete(string name)
        {
            var category = _categories.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            
            if(category != null)
                _categories.Remove(category);

            return category;
        }

        public IEnumerable<Category> Categories()
        {
            return _categories;
        }

        public Category Get(string name)
        {
            return _categories.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}