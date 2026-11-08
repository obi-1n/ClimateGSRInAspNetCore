using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// View model used when confirming email details for an external OAuth/OpenID login.
    /// </summary>
    public class ExternalLoginConfirmationViewModel
    {
        /// <summary>
        /// User's email address returned or requested from the external provider.
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for displaying available third-party authentication providers.
    /// </summary>
    public class ExternalLoginListViewModel
    {
        /// <summary>
        /// URL to redirect the user to upon successful external authentication.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// View model for sending a two-factor authentication verification code.
    /// </summary>
    public class SendCodeViewModel
    {
        /// <summary>
        /// Selected two-factor provider (e.g., SMS, Email).
        /// </summary>
        public string? SelectedProvider { get; set; }

        /// <summary>
        /// Collection of available two-factor authentication providers.
        /// </summary>
        public ICollection<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? Providers { get; set; }

        /// <summary>
        /// Destination URL after verification succeeds.
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Indicates whether the user session should be remembered across browser visits.
        /// </summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// View model for submitting and verifying a two-factor security code.
    /// </summary>
    public class VerifyCodeViewModel
    {
        /// <summary>
        /// Name of the two-factor provider used to generate the code.
        /// </summary>
        [Required]
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// The verification code entered by the user.
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Destination URL after successful code verification.
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Indicates whether to remember the current browser and bypass 2FA on future logins from this device.
        /// </summary>
        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        /// <summary>
        /// Indicates whether the authentication cookie should persist.
        /// </summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// View model for initiating account recovery by email.
    /// </summary>
    public class ForgotViewModel
    {
        /// <summary>
        /// Registered email address of the account to recover.
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for standard email and password authentication.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// User's registered email address.
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Account password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Flag indicating whether the login cookie should persist across sessions.
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// View model for new user registration, capturing name, email, and password.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// User's first name.
        /// </summary>
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name.
        /// </summary>
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Account email address used for login and notifications.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Account password satisfying length and complexity requirements.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation password to verify accurate entry.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

    /// <summary>
    /// View model for setting a new password via password reset token.
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Account email address being reset.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// New password to establish on the account.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation matching the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        /// <summary>
        /// Security reset token issued to the user's email.
        /// </summary>
        public string? Code { get; set; }
    }

    /// <summary>
    /// View model for requesting a password reset email link.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Email address associated with the forgotten account.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
