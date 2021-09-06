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
        public double Amount { get; set; } 

        [StringLength(100)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        public DateTime Date { get; set; }

        // public CategoryEntity Category { get; set; }

        // [ForeignKey(nameof(Category))]
        // [Required]
        // public int CategoryId { get; set; }
    }
}