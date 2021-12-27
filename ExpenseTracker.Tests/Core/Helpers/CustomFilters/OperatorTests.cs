using System;
using NUnit.Framework;
using ExpenseTracker.Core.Helpers.CustomFilters;
using Moq;
using System.Linq.Expressions;

namespace ExpenseTracker.Tests.Core
{
    [TestFixture]
    public class OperatorTests
    {
        #region EqualsOperator
        [Test]
        public void WhenCreatingObject_CorrectNameIsSet()
        {
            // Arrange
            EqualsOperator op = new EqualsOperator(null, null);

            // Assert
            Assert.AreEqual(op.Name, nameof(EqualsOperator));
        }        

        [Test]
        public void ToExpression_ReturnsEqualsExpression()
        {
            // Arrange
            Mock<Node> operand = new Mock<Node>();
            operand.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            Node op = new EqualsOperator(operand.Object, operand.Object);

            // Act
            var result = op.ToExpression();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.NodeType, ExpressionType.Equal);
        }

        [Test]
        public void ToExpression_CallsToExpressionOfBothOperands()
        {
            // Arrange
            Mock<Node> operand1 = new Mock<Node>();
            Mock<Node> operand2 = new Mock<Node>();
            operand1.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            operand2.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            Node op = new EqualsOperator(operand1.Object, operand2.Object);

            // Act
            var result = op.ToExpression();

            // Assert
            operand1.Verify(o => o.ToExpression(), Times.Once);
            operand2.Verify(o => o.ToExpression(), Times.Once);
        }
        #endregion
    }
}