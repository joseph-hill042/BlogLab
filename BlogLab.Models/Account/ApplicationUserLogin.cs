using System.ComponentModel.DataAnnotations;

namespace BlogLab.Models.Account
{
    public class ApplicationUserLogin
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(5, ErrorMessage = "Username must be at least 5-20 characters")]
        [MaxLength(20, ErrorMessage = "Username must be at least 5-20 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(10, ErrorMessage = "Password must be at least 10-50 characters")]
        [MaxLength(50, ErrorMessage = "Password must be at least 10-50 characters")]
        public string Password { get; set; }
    }
}