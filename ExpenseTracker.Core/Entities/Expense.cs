using System;

namespace ExpenseTracker.Core.Entities
{
    public class Expense
    {
        public int Id { get; private set; }

        public double Amount { get; private set; } 

        public string Description { get; private set; }

        public Category Category { get; set; }

        public DateTime Date { get; private set; }

        private User _user;

        public User User
        {
            get { return _user; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(User));
                if (_user != null && !_user.Email.Equals(value.Email, StringComparison.OrdinalIgnoreCase))
                    throw new NotSupportedException("Changing user is not allowed");
                _user = value;
            }
        }

        public Expense(double amount, User user, Category category = null, string description = null, DateTime date = new DateTime())
        {
            User = user;
            Amount = amount;
            Description = description;
            Category = category;
            Date = date == new DateTime() ? DateTime.Now.Date : date.Date;
        }

        public Expense(int id, double amount, User user, Category category = null,string description = null, DateTime date = new DateTime()) : this(amount, user,category, description, date)
        {
            Id = id;
        }

        public Expense(int id, Expense expense, User user) : this(expense.Amount, user, expense.Category, expense.Description, expense.Date)
        {
            Id = id;
        }
    }
}
