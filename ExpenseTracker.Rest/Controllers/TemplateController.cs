using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ExpenseTracker.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        [HttpGet]
        [Route("expense")]
        public IActionResult GetExpenseUploadTemplate()
        {
            Stream stream = new FileStream("./Templates/Expense_Upload_Template.csv", FileMode.Open, FileAccess.Read);
            if(stream == null)
                return NotFound();

            return File(stream, "text/csv");
        }
    }
}
