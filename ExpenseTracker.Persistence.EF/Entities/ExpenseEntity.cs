using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Persistence.Entities
{
    [Table("expense")]
    public class ExpenseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public double Amount { get; private set; } 

        [StringLength(100)]
        public string Description { get; private set; }

        // public CategoryEntity Category { get; set; }

        // [ForeignKey(nameof(Category))]
        // [Required]
        // public int CategoryId { get; set; }
    }
}