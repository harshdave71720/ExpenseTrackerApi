using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Rest.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        
        [MinLength(1)]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
    }
}