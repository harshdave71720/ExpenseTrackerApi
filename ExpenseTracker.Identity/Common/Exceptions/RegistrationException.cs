using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Identity.Common.Exceptions
{
    public class RegistrationException : Exception
    {
        public IEnumerable<string> Errors { get; set; }

        public RegistrationException(IEnumerable<string> errors)
        {
            Errors = errors;       
        }
    }
}
