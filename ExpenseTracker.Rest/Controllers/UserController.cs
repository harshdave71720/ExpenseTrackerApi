using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Services;
using AutoMapper;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        { 
            _userService = userService;
            _mapper = mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get(string email)
        {
            var user = await this._userService.Get(email);

            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserDto>(user));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> Add(UserDto user)
        { 
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedUser = await _userService.Add(_mapper.Map<User>(user));

            if (addedUser == null)
                return BadRequest();

            return Ok(_mapper.Map<UserDto>(addedUser));
        }
    }
}
