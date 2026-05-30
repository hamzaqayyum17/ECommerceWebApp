using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}