using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExpenseTracker.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public override string Email { get; set; }

        [Required]
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public override string UserName { get; set; }

        public ApplicationUser() : base()
        {}

        public ApplicationUser(string email, string firstname, string lastname = null) : base()
        { 
            if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstname))
                throw new ArgumentException();

            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            UserName = email;
        }
    }
}
