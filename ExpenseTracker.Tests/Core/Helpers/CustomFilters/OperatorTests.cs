using System;
using NUnit.Framework;
using ExpenseTracker.Core.Helpers.CustomFilters;
using Moq;
using System.Linq.Expressions;
using System.Collections.Generic;

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

        #region AndOperator
        
        [Test]
        public void Cosntructor_SetsNameToAndOperator()
        {
            // Arrange
            Operator andOperator = new AndOperator(new List<Node>());

            // Assert
            Assert.AreEqual(nameof(AndOperator), andOperator.Name);
        }

        [Test]
        public void ToExpression_ReturnsAndOperator()
        {
            // Arrange
            Operator andOperator = new AndOperator(new List<Node>());

            // Act
            Expression result = andOperator.ToExpression();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ExpressionType.And, result.NodeType);
        }


        [Test]
        public void ToExpression_()
        {
            // Arrange
            Mock<Node> operand1 = new Mock<Node>();
            Mock<Node> operand2 = new Mock<Node>();
            operand1.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            operand2.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            Node exp1 = new EqualsOperator(operand1.Object, operand2.Object);

            Mock<Node> operand3 = new Mock<Node>();
            Mock<Node> operand4 = new Mock<Node>();
            operand3.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            operand4.Setup(o => o.ToExpression()).Returns(Expression.Constant(1));
            Node exp2 = new EqualsOperator(operand3.Object, operand4.Object);

            Operator andOperator = new AndOperator(new List<Node> { exp1, exp2 });

            // Act
            Expression result = andOperator.ToExpression();

            // Assert
            Assert.IsNotNull(result);
            string expression = result.ToString();
            Assert.AreEqual("wrong", result.ToString());
        }
        #endregion
    }
}