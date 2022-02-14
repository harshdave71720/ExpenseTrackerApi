using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ExpenseTracker.Core.Services;
using AutoMapper;
using System.Linq;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Web.Controllers
{
    [Route("[controller]")]
    public class ExpensesController : Controller
    {
        private readonly ILogger<ExpensesController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;

        public ExpensesController(IExpenseService expenseService,
             ICategoryService categoryService,
             ILogger<ExpensesController> logger,
             IMapper mapper )
        {
            _categoryService = categoryService;
            _expenseService = expenseService;
            _logger = logger;
            _mapper = mapper;
        }
        
        //[HttpGet]
        //[Route("")]
        //public async Task<IActionResult> Index()
        //{
        //    var expenses = await _expenseService.Get();
        //    return View(expenses.Select(_mapper.Map<ExpenseViewModel>));
        //}

        [HttpGet]
        [Route("New")]
        public async Task<IActionResult> Add()
        {
            return View("ExpenseForm", new AddExpenseViewModel { Expense = new ExpenseViewModel(), 
                Categories = (await _categoryService.Get(null)).Select(_mapper.Map<CategoryViewModel>) ?? new List<CategoryViewModel>() });
        }

        //[HttpPost]
        //[Route("New")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Add(ExpenseViewModel expense)
        //{
        //    var e = _mapper.Map<Expense>(expense);
        //    var result = await _expenseService.Add(e, expense.Category);
            
        //    if(result == null)
        //        throw new Exception();

        //    return RedirectToAction(nameof(Index));
        //}

    }
}