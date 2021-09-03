using System;
using ExpenseTracker.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Rest.Dtos
{
    public class ExpenseDto
    {
        [Required]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }
    }
}