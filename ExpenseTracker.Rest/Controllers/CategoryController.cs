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

namespace ExpenseTracker.Rest.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoryController(IMapper mapper, ICategoryService categoryService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return Ok((await _categoryService.Get()).Select(_mapper.Map<CategoryDto>) ?? new List<CategoryDto>());
        }

        [HttpGet]
        [Route("{category}")]
        public async Task<IActionResult> Get(string category)
        {
            var cat = await _categoryService.Get(category);

            if(cat == null)
                return BadRequest();

            return Ok(_mapper.Map<CategoryDto>(cat));
        }

    }
}