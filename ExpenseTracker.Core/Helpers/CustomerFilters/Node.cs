using System;
using System.Linq.Expressions;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public abstract class Node{
        public abstract Expression ToExpression();
    }
}