using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Identity.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
