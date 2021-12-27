using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public abstract class AndOperator : MultiOperator
    {
        public AndOperator(List<Node> operands) : base(nameof(AndOperator), operands)
        {
        }
    }
}