using System;
using System.Linq.Expressions;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public abstract class Operand : Node
    {
        public readonly string Type;

        public Operand(string type)
        {
            Type = type;
        }
    }

    public class ConstantOperand : Operand
    {
        private string _dataType;
        private string _value;

        public ConstantOperand(string dateType, string value) : base(nameof(ConstantOperand))
        {
            _dataType = dateType;
            _value = value;
        }

        public override Expression ToExpression()
        {
            return Expression.Constant(_value);
        }
    }
}