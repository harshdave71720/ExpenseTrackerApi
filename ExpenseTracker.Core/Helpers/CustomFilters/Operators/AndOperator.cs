using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public class AndOperator : MultiOperator
    {
        public AndOperator(List<Node> operands) : base(nameof(AndOperator), operands)
        {
            _operands = operands;
        }

        public override Expression ToExpression()
        {
            return ToExpression(0);     
        }
        
        private Expression ToExpression(int operandNumber)
        {
            if(operandNumber == _operands.Count() - 1)
                return _operands[operandNumber].ToExpression();

            return Expression.And(_operands[operandNumber].ToExpression(), ToExpression(operandNumber + 1));
        }
    }
}