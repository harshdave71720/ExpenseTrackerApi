using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Core.Exceptions
{
    public class DependencyNullException : Exception
    {
        public DependencyNullException(string typeName) : base(ErrorMessages.DependencyIsNull(typeName)) { }
    }
}
