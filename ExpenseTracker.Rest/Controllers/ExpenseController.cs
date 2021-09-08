using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public ExpenseController(IExpenseRepository repository, IMapper mapper)
        {
            _expenseRepository = repository;
            _mapper = mapper;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(ExpenseDto expense)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _expenseRepository.Add(_mapper.Map<ExpenseDto, Expense>(expense));
            await _expenseRepository.SaveChangesAsync();
            return Ok(_mapper.Map<Expense, ExpenseDto>(result));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var results = await _expenseRepository.Expenses(null) ?? new List<Expense>();
            return Ok(results.Select(_mapper.Map<Expense, ExpenseDto>));
        } 

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var expense = await _expenseRepository.Get(id);
            
            if(expense == null)
                return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpGet]
        [Route("category/{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var results = await _expenseRepository.Expenses(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                            ?? new List<Expense>();

            return Ok(results.Select(_mapper.Map<Expense, ExpenseDto>));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseRepository.Delete(id);
            await _expenseRepository.SaveChangesAsync();
            if(expense == null) return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Put(ExpenseDto expense)
        {
            var result = await _expenseRepository.Update(_mapper.Map<Expense>(expense));

            if(result == null)
                return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(result));
        }
    }
}