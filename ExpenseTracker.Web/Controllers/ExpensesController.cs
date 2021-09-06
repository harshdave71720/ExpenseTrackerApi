using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace ExpenseTracker.Web.Controllers
{
    [Route("[controller]")]
    public class ExpensesController : Controller
    {
        private readonly ILogger<ExpensesController> _logger;
        private static readonly HttpClient _httpClient = new HttpClient();

        public ExpensesController(ILogger<ExpensesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:5001/Expense");
            response.EnsureSuccessStatusCode();
            var expenses = JsonConvert.DeserializeObject<IEnumerable<ExpenseViewModel>>(await response.Content.ReadAsStringAsync());
            return View(expenses);
        }

        [HttpGet]
        [Route("New")]
        public async Task<IActionResult> Add()
        {
            return View(new ExpenseViewModel());
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> Add(ExpenseViewModel expense)
        {
            return RedirectToAction("Index");
        }
    }
}