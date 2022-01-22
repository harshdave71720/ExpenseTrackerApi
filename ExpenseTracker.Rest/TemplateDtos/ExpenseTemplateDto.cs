using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Rest.TemplateDtos
{
    public class ExpenseTemplateDto
    {
        [Required]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string CategoryName { get; set; }
    }
}
