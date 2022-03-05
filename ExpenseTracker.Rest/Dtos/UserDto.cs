using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Rest.Dtos
{
    public class UserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
