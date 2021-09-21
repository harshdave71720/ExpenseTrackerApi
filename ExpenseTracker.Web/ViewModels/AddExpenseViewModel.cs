using System.Collections.Generic;

namespace ExpenseTracker.Web.ViewModels
{
    public class AddExpenseViewModel
    {
        public ExpenseViewModel Expense { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}