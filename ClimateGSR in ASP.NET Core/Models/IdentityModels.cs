using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Application user entity representing an authenticated user, extending ASP.NET Core Identity's <see cref="IdentityUser"/>.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Keep the original score column so upgrading older databases cannot drop it.
        // New game scores use Member.Highscore; the leaderboard honours both versions.
        public int HighScore { get; set; }

        /// <summary>
        /// User's first name.
        /// </summary>
        [StringLength(100)] public string? FirstName { get; set; }

        /// <summary>
        /// User's last name or surname.
        /// </summary>
        [StringLength(100)] public string? LastName { get; set; }

        /// <summary>
        /// Timestamp when the user registered.
        /// </summary>
        public string? RegisteredAt { get; set; }
    }

    /// <summary>
    /// Adds FirstName / LastName claims to the user's cookie principal at sign-in.
    /// Replaces the MVC 5 <c>GenerateUserIdentityAsync</c> method.
    /// </summary>
    public class ApplicationUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                identity.AddClaim(new Claim("FirstName", user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                identity.AddClaim(new Claim("LastName", user.LastName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
            }

            return identity;
        }
    }

    /// <summary>
    /// Extension methods for <see cref="IIdentity"/> to retrieve custom user claims like first name and last name.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Retrieves the user's first name claim from the current identity, falling back to the username if unavailable.
        /// </summary>
        public static string GetFirstName(this IIdentity? identity)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                var claim = claimsIdentity.FindFirst("FirstName") ?? claimsIdentity.FindFirst(ClaimTypes.GivenName);
                if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    return claim.Value;
                }
            }

            return identity?.Name ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the user's last name claim from the current identity.
        /// </summary>
        public static string GetLastName(this IIdentity? identity)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                var claim = claimsIdentity.FindFirst("LastName") ?? claimsIdentity.FindFirst(ClaimTypes.Surname);
                if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    return claim.Value;
                }
            }

            return string.Empty;
        }
    }
}
