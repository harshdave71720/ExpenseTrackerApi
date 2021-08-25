using System;
using System.Collections.Generic;

namespace ExpenseTracker.Core.Entities
{
    public class Category
    {
        public string Name { get; private set; }

        public List<Expense> Expenses { get; set; }

        public Category(string name)
        {
            Name = name;
        }
    }
}