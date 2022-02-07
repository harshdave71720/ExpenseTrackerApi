using System;
using NUnit.Framework;
using ExpenseTracker.Rest.Controllers;
using Moq;
using ExpenseTracker.Core.Services;
using AutoMapper;
using ExpenseTracker.Rest.MapperProfiles;
using System.Collections.Generic;
using ExpenseTracker.Core.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Rest.Dtos;

namespace ExpenseTracker.Tests.Rest
{
    [TestFixture]
    public class ExpenseControllerTests
    {
        private static IMapper _mapper;

        public static void OneTimeSetup()
        {
            _mapper = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<MapperProfile>();
            }).CreateMapper();
        }

        public ExpenseControllerTests()
        {
            ExpenseControllerTests.OneTimeSetup();
        }

        //[Test]
        //public async Task Test1()
        //{
        //    Mock<IExpenseService> expenseService = new Mock<IExpenseService>();
        //    expenseService.Setup(x => x.Get()).Returns(Task.FromResult((IEnumerable<Expense>)Expenses));
            
        //    var contoller = new ExpenseController(expenseService.Object, null,_mapper);

        //    var result = (OkObjectResult)await contoller.Get();
        //    Assert.IsNotNull(result);

        //    IEnumerable<ExpenseDto> expensesToAssert = (IEnumerable<ExpenseDto>)result.Value;
        //    Assert.IsNotNull(expensesToAssert);
        //    AssertExpenses(expectedExpenses : Expenses, expensesToAssert);
        //}

        private static User user = new User(1, "Demo User", "Demo", "Demo");

        private static List<Expense> Expenses = new List<Expense>
        {
            new Expense(1, 1, user, Category, "SomeDescription", DateTime.Now.Date),
            new Expense(2, 1.1, user, null, "SomeDescription", DateTime.Now.Date.AddDays(1)),
            new Expense(3, 1.1, user, Category, null),
            new Expense(4, 1.1, user, null, null, DateTime.Now.Date.AddDays(-1)),
        };

        private static Category Category = new Category(1,"Category123", null);

        private void AssertExpenses(IEnumerable<Expense> expectedExpenses, IEnumerable<ExpenseDto> expensesToAssert)
        {
            Assert.AreEqual(expectedExpenses.Count(), expensesToAssert.Count());
            foreach(var expenseToAssert in expensesToAssert)
            {
                Assert.IsNotNull(expenseToAssert);
                var expectedExpense = expectedExpenses.Single(e => e.Id == expenseToAssert.Id);
                AssertExpense(_mapper.Map<ExpenseDto>(expectedExpense), expenseToAssert);
            }
        }

        private void AssertExpense(ExpenseDto expectedExpense, ExpenseDto expenseToAssert)
        {
            Assert.AreEqual(expectedExpense.Id, expenseToAssert.Id);
            Assert.AreEqual(expectedExpense.Amount, expenseToAssert.Amount);
            Assert.AreEqual(expectedExpense.Date, expenseToAssert.Date);
            Assert.AreEqual(expectedExpense.Description, expenseToAssert.Description);
            Assert.AreEqual(expectedExpense.CategoryName, expenseToAssert.CategoryName);
        }
    }
}