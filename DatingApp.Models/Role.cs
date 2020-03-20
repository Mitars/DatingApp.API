using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.Models
{
    /// <summary>
    /// The role class.
    /// </summary>
    public class Role : IdentityRole<int>
    {
        /// <summary>
        /// Gets or sets the available user roles.
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}