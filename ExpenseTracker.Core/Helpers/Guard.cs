using System;
using ExpenseTracker.Core.Exceptions;

namespace ExpenseTracker.Core.Helpers
{
    public static class Guard
    {
        public static void AgainstDependencyNull<T>(T dependency) 
        {
            if(dependency == null)
                throw new DependencyNullException(typeof(T).FullName);
        }
        public static void AgainstNull(object obj, string paramName)
        {
            if(obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static void AgainstNullOrEmpty(string value, string paramName)
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentException("String cannot be null or empty", paramName);
        }
        
        public static void AgainstNullOrWhiteSpace(string value, string paramName)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("String cannot be null or empty", paramName);
        }

        public static void AgainstZeroOrNegative(int number, string paramName)
        {
            if(number <= 0)
                throw new ArgumentException("Value must be positive", nameof(paramName));
        }

        public static void AgainstNegative(int number, string paramName)
        {
            if(number < 0)
                throw new ArgumentException("Value must be non negative", nameof(paramName));
        }
    }
}