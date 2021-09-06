using System;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Rest.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}