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
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using ExpenseTracker.Rest.Models;

namespace ExpenseTracker.Rest.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : AppControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoryController(IMapper mapper, ICategoryService categoryService, IUserService userService)
        : base(userService)
        {
            Guard.AgainstDependencyNull(mapper);
            Guard.AgainstDependencyNull(categoryService);
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var categories = await _categoryService.Get(await GetUser());
            return OkResponseResult(categories.Select(_mapper.Map<CategoryDto>));
        }

        [HttpGet]
        [Route("{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var cat = await _categoryService.Get(await GetUser(), category);

            if (cat == null)
                throw new NotFoundException(ErrorMessages.CategoryNotFound(category));

            return OkResponseResult(_mapper.Map<CategoryDto>(cat));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(CategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequestResponseFromModelState();
            }

            Category category = _mapper.Map<Category>(categoryDto);
            category.User = await GetUser();
            var addedCategory = await _categoryService.Add(category);
            return CreatedResponseResult(_mapper.Map<CategoryDto>(addedCategory));
        }

        [HttpDelete]
        [Route("{category}")]
        public async Task<IActionResult> Delete(string category)
        {
            var deletedCategory = await _categoryService.Delete(await GetUser(), category);
            return OkResponseResult(_mapper.Map<CategoryDto>(deletedCategory));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Put(CategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequestResponseFromModelState();
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.User = await GetUser();
            var updatedCategory = await _categoryService.Update(category);
            return OkResponseResult(_mapper.Map<CategoryDto>(updatedCategory));
        }
    }
}