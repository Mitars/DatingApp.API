using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Models
{
    /// <summary>
    /// The user role class.
    /// </summary>
    public class UserRole : IdentityUserRole<int>
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public Role Role { get; set; }
    }
}