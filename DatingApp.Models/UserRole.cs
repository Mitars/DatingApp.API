using Microsoft.AspNetCore.Identity;

namespace DatingApp.Models
{
    /// <summary>
    /// The user role class.
    /// </summary>
    public class UserRole : IdentityUserRole<int>
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public virtual Role Role { get; set; }
    }
}