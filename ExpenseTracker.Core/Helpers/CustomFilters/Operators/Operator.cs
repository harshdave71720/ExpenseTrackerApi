using System;
using System.Collections.Generic;

namespace ExpenseTracker.Core.Helpers.CustomFilters
{
    public abstract class Operator : Node
    {
        public readonly string Name;

        public Operator(string name)
        {
            Name = name;
        }
    }

    public abstract class BinaryOperator : Operator
    {
        protected Node _operand1;
        protected Node _operand2;

        public BinaryOperator(string name, Node operand1, Node operand2) : base(name)
        {
            _operand1 = operand1;
            _operand2 = operand2;
        }
    }
    
    public abstract class MultiOperator : Operator
    {
        protected List<Node> _operands;

        public MultiOperator(string name, List<Node> operands) : base(name)
        {
            _operands = operands;
        }
    }
}