using System;

namespace ExpenseTracker.Core.Entities
{
    public class Expense
    {
        public double Amount { get; private set; } 

        public string Description { get; private set; }
    }
}
