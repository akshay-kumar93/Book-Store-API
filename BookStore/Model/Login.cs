using System.ComponentModel.DataAnnotations;

namespace BookStore.Model
{
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}
