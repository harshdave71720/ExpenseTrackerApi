using System;
using System.Collections.Generic;

namespace ExpenseTracker.Core.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        
        public string Name { get; private set; }

        public List<Expense> Expenses { get; private set; }

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

        public Category(int id, string name,User user = null, List<Expense> expenses = null)
        {
            Id = id;
            Name = name;
            Expenses = expenses;
            if (user != null)
                User = user;
        }
    }
}