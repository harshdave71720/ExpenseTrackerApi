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
using ExpenseTracker.Rest.Models;
using System.Linq;
using ExpenseTracker.Core.Helpers;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class UserController : AppControllerBase
    {
        private readonly IIdentiyUserService _identityUserService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper, IIdentiyUserService IdentityUserService)
            : base(userService)
        {
            Guard.AgainstDependencyNull(mapper);
            Guard.AgainstDependencyNull(IdentityUserService);

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

            return OkResponseResult(_mapper.Map<UserDto>(user));
        }

        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequestResponseFromModelState();

            User addedUser = null;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _identityUserService.Register(user);
                addedUser = await _userService.Add(_mapper.Map<User>(user));

                scope.Complete();
            }

            return CreatedResponseResult(_mapper.Map<UserDto>(addedUser));
        }

        [Route("token")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody]LoginUserDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromModelState();
            }
                
            var token = await _identityUserService.GetToken(user);
            return OkResponseResult(new { Token = token });
        }
    }
}
