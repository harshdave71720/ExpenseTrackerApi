using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Identity.Entities
{
    public class ApplicationUser
    {
        public string Email { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public ApplicationUser(string email, string firstname, string lastname = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstname))
                throw new ArgumentException();

            Email = email;
            Firstname = firstname;
            Lastname = lastname;
        }
    }
}
