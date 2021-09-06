using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Rest.Dtos;
using AutoMapper;
using System.Linq;

namespace ExpenseTracker.Rest.Controller
{
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        // private readonly IMapper _mapper;
        // private readonly ICategoryRepository _categoryRepository;

        // public CategoryController(IMapper mapper, ICategoryRepository categoryRepository)
        // {
        //     _mapper = mapper;
        //     _categoryRepository = categoryRepository;
        // }

        // [HttpGet]
        // [Route("")]
        // public async Task<IActionResult> Get()
        // {
        //     return Ok(_categoryRepository.Categories().Select(c => _mapper.Map<CategoryDto>(c)));
        // }

        // [HttpPost]
        // [Route("")]
        // public async Task<IActionResult> Post(CategoryDto c)
        // {
        //     if(!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     return Ok(_mapper.Map<CategoryDto>(_categoryRepository.Add(_mapper.Map<Category>(c))));
        // }
    }
}