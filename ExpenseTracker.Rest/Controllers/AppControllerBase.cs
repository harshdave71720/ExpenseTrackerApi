using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Exceptions;
using ExpenseTracker.Core.Helpers;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Rest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpenseTracker.Rest.Controllers
{
    [ApiController]
    public class AppControllerBase : ControllerBase
    {
        protected readonly IUserService _userService;

        public AppControllerBase(IUserService userService)
        {
            Guard.AgainstDependencyNull(userService);
            _userService = userService;
        }

        protected virtual async Task<User> GetUser()
        {
            var user = await _userService.Get(this?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound(this?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value));
            return user;
        }

        protected virtual IActionResult BadRequestResponseFromModelState()
        {
            return BadRequest
            (
                new Response
                (
                    StatusCodes.Status400BadRequest,
                    errors : ModelState
                        .Where(e => e.Value?.Errors?.Count() > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage)
                )
            );
        }

        protected virtual IActionResult BadRequestResponse(string error)
        {
            return BadRequest(new Response(StatusCodes.Status400BadRequest));
        }

        protected virtual IActionResult OkResponseResult() 
        {
            return Ok(new Response(StatusCodes.Status200OK));
        }

        protected virtual IActionResult OkResponseResult(object value)
        {
            return Ok(new Response(StatusCodes.Status200OK, value));
        }

        protected virtual IActionResult CreatedResponseResult(object value)
        {
            var response = new Response(StatusCodes.Status201Created, value);
            return StatusCode(response.StatusCode, response);
        }
    }
}
