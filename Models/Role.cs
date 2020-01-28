using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Models
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