using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExpenseTracker.Identity.Infrastructure.Entities
{
    public class ApplicationUserDbEntity : IdentityUser
    {
        [Required]
        public override string Email { get; set; }

        [Required]
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public ApplicationUserDbEntity() : base()
        {
            this.UserName = Email;
        }
    }
}
