using System.ComponentModel.DataAnnotations;

namespace Canopy.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "UsernameLength")]
        [RegularExpression(@"^[a-zA-Z0-9._]+$",
            ErrorMessage = "UsernameInvalid")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailInvalid")]
        [StringLength(100, ErrorMessage = "EmailLength")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "PasswordLength")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$",
            ErrorMessage = "PasswordComplexity")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "TermsRequired")]
        [Range(typeof(bool), "true", "true",
            ErrorMessage = "TermsRequired")]
        [Display(Name = "AcceptTerms")]
        public bool AcceptTerms { get; set; }
    }
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}