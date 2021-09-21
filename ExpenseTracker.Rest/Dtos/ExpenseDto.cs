using System;
using ExpenseTracker.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Rest.Dtos
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        public DateTime? Date { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string CategoryName { get; set; }

        public ExpenseDto()
        {
            Date = DateTime.Now.Date;
        }
    }
}