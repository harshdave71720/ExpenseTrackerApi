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
            return Ok(_mapper.Map<Expense, ExpenseDto>(result));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var results = await _expenseRepository.Expenses(null) ?? new List<Expense>();
            return Ok(results.Select(_mapper.Map<Expense, ExpenseDto>));
        } 
    }
}