using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> ValidationErrors { get; private set; }
        public ValidationException(string validationError) 
        { 
            ValidationErrors = new List<string> { validationError };
        }

        public ValidationException(IEnumerable<string> validationErrors)
        { 
            ValidationErrors = validationErrors;
        }
    }
}
