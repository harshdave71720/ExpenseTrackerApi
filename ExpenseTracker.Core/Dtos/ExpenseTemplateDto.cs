using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExpenseTracker.Core.Dtos
{
    internal class ExpenseTemplateDto
    {
        [Required]
        public double Amount { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public ExpenseTemplateDto()
        {
            Date = DateTime.Now.Date;
        }
    }
}
