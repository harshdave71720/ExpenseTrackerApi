using System;
using System.Linq.Expressions;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public class EqualsOperator : BinaryOperator
    {
        public EqualsOperator(Node operand1, Node operand2) : base(nameof(EqualsOperator), operand1, operand2)
        {}

        public override Expression ToExpression()
        {
            return Expression.Equal(_operand1.ToExpression(), _operand2.ToExpression());
        }
    }
}