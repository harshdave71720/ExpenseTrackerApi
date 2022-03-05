using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExpenseTracker.Core.Services;
using AutoMapper;
using ExpenseTracker.Rest.Dtos;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Identity.Dtos;
using ExpenseTracker.Identity.Services;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IIdentiyUserService _identityUserService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper, IIdentiyUserService IdentityUserService)
        { 
            _userService = userService;
            _mapper = mapper;
            _identityUserService = IdentityUserService;
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

        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserDto user)
        { 
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            User addedUser = null;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _identityUserService.Register(user);
                addedUser = await _userService.Add(_mapper.Map<User>(user));

                scope.Complete();
            }

            //await _identityUserService.Register(user);
            //addedUser = await _userService.Add(_mapper.Map<User>(user));

            if (addedUser == null)
                return BadRequest();

            return Ok(_mapper.Map<UserDto>(addedUser));
        }

        [Route("token")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody]LoginUserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _identityUserService.GetToken(user);

            if(token == null)
                return Forbid();

            return Ok(new { Token = token });
        }
    }
}
