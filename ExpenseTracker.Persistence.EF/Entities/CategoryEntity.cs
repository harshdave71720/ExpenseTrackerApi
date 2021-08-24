using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ExpenseTracker.Persistence.Entities
{
    public class CategoryEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; private set; }

        public List<ExpenseEntity> Expenses { get; set; }
    }
}