using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Core.Services;
using Microsoft.AspNetCore.Http;
using ExpenseTracker.Rest.TemplateDtos;
using System.Security.Claims;
using ExpenseTracker.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Core.Exceptions;
using ExpenseTracker.Rest.Models;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ExpenseController : AppControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ITemplateService _templateService;

        public ExpenseController(IExpenseService expenseService, ITemplateService templateService,IMapper mapper, ICategoryService categoryService ,IUserService userService)
        : base(userService)
        {
            Guard.AgainstNull(expenseService, nameof(expenseService));
            Guard.AgainstNull(templateService, nameof(expenseService));
            Guard.AgainstNull(mapper, nameof(expenseService));
            Guard.AgainstNull(mapper, nameof(categoryService));

            _mapper = mapper;
            _expenseService = expenseService;
            _templateService = templateService;
            _categoryService = categoryService;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(ExpenseDto expense)
        {
            if(!ModelState.IsValid)
                return BadRequestResponseFromModelState();

            var expenseToAdd = _mapper.Map<Expense>(expense);
            expenseToAdd.User = await GetUser();

            if (expense.CategoryName != null)
            {
                var category = await _categoryService.Get(await GetUser(), expense.CategoryName);
                if (category == null)
                    throw new NotFoundException(ErrorMessages.CategoryNotFound(expense.CategoryName));
                expenseToAdd.Category = category;
            }

            var createdExpense = await _expenseService.Add(expenseToAdd);
            return CreatedResponseResult(_mapper.Map<ExpenseDto>(createdExpense));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var results = await _expenseService.Get(await GetUser());
            return OkResponseResult(results.Select(_mapper.Map<ExpenseDto>));
        } 

        [HttpGet]
        [Route("GetPaged")]
        public async Task<IActionResult> Get(int limit, int offset, bool latestFirst)
        {
            var results = await _expenseService.GetAll(await GetUser(), null, limit, offset, latestFirst);
            return OkResponseResult(results.Select(_mapper.Map<ExpenseDto>));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var expense = await _expenseService.Get(await GetUser(), id);
            
            if(expense == null)
                return NotFound();

            return OkResponseResult(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpGet]
        [Route("category/{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var results = await _expenseService.Get(await GetUser(), category);

            if (results == null)
                return NotFound();

            return OkResponseResult(results.Select(_mapper.Map<ExpenseDto>));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseService.Delete(await GetUser(), id);
            return OkResponseResult(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Put(ExpenseDto expense)
        {
            if(!ModelState.IsValid)
                return BadRequestResponseFromModelState();

            var expenseToUpdate = _mapper.Map<Expense>(expense);
            expenseToUpdate.User = await GetUser();
            if (expense.CategoryName != null)
                expenseToUpdate.Category = await _categoryService.Get(await GetUser(), expense.CategoryName);

            var updatedExpense = await _expenseService.Update(expenseToUpdate);

            return OkResponseResult(_mapper.Map<ExpenseDto>(updatedExpense));
        }

        [HttpGet]
        [Route("Count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _expenseService.GetExpenseCount(await GetUser());
            return OkResponseResult(new Response(StatusCodes.Status200OK, count));
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            Guard.AgainstNull(file, nameof(file));
            var expenses = await this._templateService.GetRecordsFromTemplate<ExpenseTemplateDto>(file.OpenReadStream());
            var expensesWithCategoies = expenses.Select( e => new KeyValuePair<Expense, string>(
                    _mapper.Map<Expense>(e),
                    e.CategoryName
                )
            );

            await _expenseService.Add(await GetUser(), expensesWithCategoies);

            return OkResponseResult();
        }
    }
}