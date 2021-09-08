using System;
using System.Collections.Generic;

namespace ExpenseTracker.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        
        public string Name { get; private set; }

        public List<Expense> Expenses { get; private set; }

        public Category(string name, List<Expense> expenses)
        {
            Name = name;
            Expenses = expenses;
        }
    }
}