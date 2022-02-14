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
using ExpenseTracker.Core.Repositories;
using System.Security.Claims;
using ExpenseTracker.Core.Helpers;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;
        private readonly ITemplateService _templateService;
        private readonly IUserRepository _userRepository;
        //private string UserEmail => this.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        private string UserEmail => "harshdave71720@gmail.com";

        public ExpenseController(IExpenseService expenseService, ITemplateService templateService,IMapper mapper)
        {
            Guard.AgainstNull(expenseService, nameof(expenseService));
            Guard.AgainstNull(templateService, nameof(expenseService));
            Guard.AgainstNull(mapper, nameof(expenseService));

            _mapper = mapper;
            _expenseService = expenseService;
            _templateService = templateService;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(ExpenseDto expense)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _expenseService.Add(UserEmail, _mapper.Map<Expense>(expense), expense.CategoryName);
            return Ok(_mapper.Map<ExpenseDto>(result));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var results = await _expenseService.Get(UserEmail);
            return Ok(results.Select(_mapper.Map<ExpenseDto>));
        } 

        [HttpGet]
        [Route("GetPaged")]
        public async Task<IActionResult> Get(int limit, int offset, bool latestFirst)
        {
            var results = await _expenseService.GetAll(UserEmail, null, limit, offset, latestFirst);
            return Ok(results.Select(_mapper.Map<ExpenseDto>));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var expense = await _expenseService.Get(UserEmail, id);
            
            if(expense == null)
                return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpGet]
        [Route("category/{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var results = await _expenseService.Get(UserEmail, category);

            if (results == null)
                return NotFound();

            return Ok(results.Select(_mapper.Map<ExpenseDto>));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseService.Delete(UserEmail, id);
            if(expense == null) return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Put(ExpenseDto expense)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var result = await _expenseService.Update(UserEmail, _mapper.Map<Expense>(expense),  expense.CategoryName);

            if(result == null)
                return NotFound();

            return Ok(_mapper.Map<ExpenseDto>(result));
        }

        [HttpGet]
        [Route("Count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _expenseService.GetExpenseCount(UserEmail);
            return Ok(count);
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var expenses = await this._templateService.GetRecordsFromTemplate<ExpenseTemplateDto>(file.OpenReadStream());
            var expensesWithCategoies = expenses.Select( e => new KeyValuePair<Expense, string>(
                    _mapper.Map<Expense>(e),
                    e.CategoryName
                )
            );

            var errors = await _expenseService.Add(UserEmail, expensesWithCategoies);
            if (errors?.Count() > 0)
                return BadRequest(errors);

            return Ok();
        }
    }
}