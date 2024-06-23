using System.ComponentModel.DataAnnotations;

namespace BookStore.Model
{
    public class SignInDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
