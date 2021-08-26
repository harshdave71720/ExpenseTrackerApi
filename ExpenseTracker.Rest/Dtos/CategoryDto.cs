using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Rest.Dtos
{
    public class CategoryDto
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
    }
}