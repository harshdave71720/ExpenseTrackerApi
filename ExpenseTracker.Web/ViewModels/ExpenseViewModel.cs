using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Web.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string Category { get; set; }
    }
}