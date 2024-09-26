using System.ComponentModel.DataAnnotations;

namespace Route.PL.Models
{
    public class ResetPasswordVM
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password Not Match")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
