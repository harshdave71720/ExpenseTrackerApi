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

        public DateTime Date { get; set; }

        public CategoryEntity Category { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
    }
}