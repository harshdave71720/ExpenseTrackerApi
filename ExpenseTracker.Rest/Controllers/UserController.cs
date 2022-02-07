using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Repositories;
using AutoMapper;
using ExpenseTracker.Rest.Dtos;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        { 
           _userRepository = userRepository;
            _mapper = mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get(string email)
        {
            var user = _mapper.Map<UserDto>(await this._userRepository.GetUser(email));

            if(user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
