using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class RegistraionModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 20 characters")]
        public string? Name { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one letter and one number, and be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string? ConfirmPassword { get; set; }
    }
}
