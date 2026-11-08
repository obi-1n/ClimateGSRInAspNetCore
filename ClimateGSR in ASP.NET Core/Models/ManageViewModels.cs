using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// View model for displaying account management dashboard status.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Indicates whether the user account has a local password set.
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        /// List of external login providers linked to this account.
        /// </summary>
        public IList<UserLoginInfo>? Logins { get; set; }

        /// <summary>
        /// User's confirmed phone number, if configured.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Indicates whether two-factor authentication is active.
        /// </summary>
        public bool TwoFactor { get; set; }

        /// <summary>
        /// Indicates whether the current browser client is remembered for two-factor authentication.
        /// </summary>
        public bool BrowserRemembered { get; set; }
    }

    /// <summary>
    /// View model for managing third-party external logins linked to the user account.
    /// </summary>
    public class ManageLoginsViewModel
    {
        /// <summary>
        /// External logins currently linked to the user account.
        /// </summary>
        public IList<UserLoginInfo>? CurrentLogins { get; set; }

        /// <summary>
        /// External authentication providers available for linking.
        /// </summary>
        public IList<AuthenticationScheme>? OtherLogins { get; set; }
    }

    /// <summary>
    /// View model representing a two-factor authentication factor purpose.
    /// </summary>
    public class FactorViewModel
    {
        /// <summary>
        /// Purpose or description of the authentication factor.
        /// </summary>
        public string? Purpose { get; set; }
    }

    /// <summary>
    /// View model for establishing a local password for users who registered via external login.
    /// </summary>
    public class SetPasswordViewModel
    {
        /// <summary>
        /// The new password to set on the account.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation matching the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

    /// <summary>
    /// View model for updating the user's current password.
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// Current account password for security verification.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// The new password replacing the existing password.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation matching the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

    /// <summary>
    /// View model for modifying personal profile information including name, email, phone, and optional password.
    /// </summary>
    public class EditAccountViewModel
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
        /// User's primary contact and login email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's contact phone number.
        /// </summary>
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Current password required when confirming profile changes or changing password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? OldPassword { get; set; }

        /// <summary>
        /// Optional new password to change during profile edit.
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password (optional)")]
        public string? NewPassword { get; set; }

        /// <summary>
        /// Confirmation for the optional new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

    /// <summary>
    /// View model for permanent account deletion requests.
    /// </summary>
    public class DeleteAccountViewModel : IValidatableObject
    {
        /// <summary>
        /// Explicit checkbox confirmation acknowledging permanent account deletion.
        /// </summary>
        [Display(Name = "I understand that deleting my account is permanent and cannot be undone.")]
        public bool ConfirmDelete { get; set; }

        /// <summary>
        /// User's password required to authorize account deletion if a password exists.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string? Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ConfirmDelete)
            {
                yield return new ValidationResult(
                    "You must confirm account deletion to proceed.",
                    new[] { nameof(ConfirmDelete) });
            }
        }
    }
}