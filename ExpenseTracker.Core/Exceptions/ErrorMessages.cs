using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Core.Exceptions
{
    public static class ErrorMessages
    {
        public static string ExpenseNotFound(int id) => $"Expense with Id : { id } does not exist.";

        public static string CategoryNotFound(string name) => $"Category with Name : { name } does not exist.";

        public static string UserNotFound(string email) => $"User with Email : { email } does not exist.";

        public static string DependencyIsNull(string typeName) => $"Null was provided for the type { typeName }";

        public static string CategoryAlreadyExists(string name) => $"Category with Name : { name } already exists";

        public static string UserAlreadyExists(string email) => $"User with Email : { email } already exists";
    }
}
