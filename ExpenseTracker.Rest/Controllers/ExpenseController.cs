using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Core.Services;

namespace ExpenseTracker.Core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;

        public ExpenseController(IExpenseService expenseService, IMapper mapper)
        {
            _mapper = mapper;
            _expenseService = expenseService;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(ExpenseDto expense)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _expenseService.Add(_mapper.Map<ExpenseDto, Expense>(expense), expense.CategoryName);
            return Ok(_mapper.Map<Expense, ExpenseDto>(result));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var results = await _expenseService.Get();
            return Ok(results.Select(_mapper.Map<Expense, ExpenseDto>));
        } 

        [HttpGet]
        [Route("GetPaged")]
        public async Task<IActionResult> Get(int limit, int offset, bool latestFirst)
        {
            var results = await _expenseService.GetAll(null, limit, offset, latestFirst);
            return Ok(results.Select(_mapper.Map<Expense, ExpenseDto>));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var expense = await _expenseService.Get(id);
            
            if(expense == null)
                return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpGet]
        [Route("category/{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var results = await _expenseService.Get(category);

            return Ok(results.Select(_mapper.Map<Expense, ExpenseDto>));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseService.Delete(id);
            if(expense == null) return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Put(ExpenseDto expense)
        {
            var result = await _expenseService.Update(_mapper.Map<Expense>(expense),  expense.CategoryName);

            if(result == null)
                return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(result));
        }

        [HttpGet]
        [Route("Count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _expenseService.GetExpenseCount();
            return Ok(count);
        }
    }
}