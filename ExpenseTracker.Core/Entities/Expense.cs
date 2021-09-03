using System;

namespace ExpenseTracker.Core.Entities
{
    public class Expense
    {
        public int Id { get; private set; }

        public double Amount { get; private set; } 

        public string Description { get; private set; }

        public string Category { get; private set; }

        public Expense(double amount, string description = null, string category = null)
        {
            Amount = amount;
            Description = description;
            Category = category;
        }

        public Expense(int id, double amount, string description = null, string category = null) : this(amount, description, category)
        {
            Id = id;
        }

        public Expense(int id, Expense expense) : this(expense.Amount, expense.Description, expense.Category)
        {
            Id = id;
        }

        public string ToCsv()
        {
            return $"{Id},{Amount},{Description ?? string.Empty},{Category ?? string.Empty}";
        }

        public static string HeadersToCsv()
        {
            return $"{nameof(Id)},{nameof(Amount)},{nameof(Description)},{nameof(Category)}";
        }

        public static Expense FromCsv(string expenseString)
        {
            string[] properties = expenseString.Split(',');
            return new Expense(int.Parse(properties[0]), double.Parse(properties[1]), properties[2], properties[3]);
        }
    }
}
