using System;
using System.Linq.Expressions;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public class MemberOperand : Operand
    {
        private Expression _parameter;
        private string _memberName;

        public MemberOperand(Expression parameter, string memberName) : base(nameof(MemberOperand))
        {
            _parameter = parameter;
            _memberName = memberName;
        }

        public override Expression ToExpression()
        {
            return Expression.PropertyOrField(_parameter, _memberName);
        }
    }
}