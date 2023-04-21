using System.ComponentModel.DataAnnotations;

namespace BlogLab.Models.Account
{
    public class ApplicationUserCreate : ApplicationUserLogin
    {
        [MinLength(10, ErrorMessage = "Full Name must be at least 10-30 characters")]
        [MaxLength(30, ErrorMessage = "Full Name must be at least 10-30 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(30, ErrorMessage = "Email must be at most 30 characters")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
    }
}