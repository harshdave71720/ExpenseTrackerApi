using System;

namespace ExpenseTracker.Core.Helpers
{
    public static class Guard
    {
        public static void AgainstNull(object obj, string paramName)
        {
            if(obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static void AgainstNullOrEmpty(string param, string paramName)
        {
            if(string.IsNullOrEmpty(param))
                throw new ArgumentException("String cannot be null or empty", paramName);
        }
        
        public static void AgainstNullOrWhiteSpace(string param, string paramName)
        {
            if(string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("String cannot be null or empty", paramName);
        }

        public static void AgainstZeroOrNegative(int number, string paramName)
        {
            if(number <= 0)
                throw new ArgumentException("Value must be positive", nameof(paramName));
        }
    }
}