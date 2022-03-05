using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.Dtos;
using AutoMapper;
using System.Linq;
using ExpenseTracker.Core.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseTracker.Rest.Controller
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IUserRepository _userRepository;
        private User _user;

        public CategoryController(IMapper mapper, ICategoryService categoryService, IUserRepository userRepository)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return Ok((await _categoryService.Get(await GetUser())).Select(_mapper.Map<CategoryDto>) ?? new List<CategoryDto>());
        }

        [HttpGet]
        [Route("{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var cat = await _categoryService.Get(await GetUser(), category);

            if(cat == null)
                return BadRequest();

            return Ok(_mapper.Map<CategoryDto>(cat));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(CategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser();
            if(await _categoryService.Get(user, categoryDto.Name) != null)
                return BadRequest();

            Category category = _mapper.Map<Category>(categoryDto);
            category.User = user;
            return Ok(_mapper.Map<CategoryDto>(await _categoryService.Add(category)));
        }

        [HttpDelete]
        [Route("{category}")]
        public async Task<IActionResult> Delete(string category)
        {
            if(string.IsNullOrWhiteSpace(category))
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<CategoryDto>(await _categoryService.Delete(await GetUser(), category)));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Put(CategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.User = await GetUser();
            return Ok(_mapper.Map<CategoryDto>(await _categoryService.Update(category)));
        }

        private async Task<User> GetUser()
        {
            if(_user == null)
                _user = await _userRepository.GetUser(this.User.Claims.First(c => c.Type == ClaimTypes.Email).Value);

            return _user;
        }
    }
}