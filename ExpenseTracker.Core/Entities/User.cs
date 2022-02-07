using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Core.Entities
{
    public class User
    {
        public int Id { get; private set; }

        public string Email { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public User(int id, string email, string firstName, string lastName)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
