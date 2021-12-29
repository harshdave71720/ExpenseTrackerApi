using System;
using NUnit.Framework;
using ExpenseTracker.Core.Helpers.CustomFilters;
using System.Linq.Expressions;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    public class OperandTests
    {
        #region MemberOperand
        
        [Test]
        public void ToExpression_ReturnsAMemberOperand()
        {
            // Arrange
            var operand = new MemberOperand(Expression.Parameter(typeof(Sample), "s"), "Prop1");

            // Act
            Expression result = operand.ToExpression();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ExpressionType.MemberAccess, result.NodeType);
        }
    }

    class Sample
    {
        public int Prop1 { get; set; }
        public int Field1;
        public string SampleName;
    }
}