using System.ComponentModel.DataAnnotations;

namespace Route.PL.Models
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
    }
}
