using System.Collections.Generic;

namespace DatingApp.Business.Dtos
{
    /// <summary>
    /// The user with roles.
    /// </summary>
    public class UserWithRoles
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the list of roles.
        /// </summary>
        public IEnumerable<string> Roles { get; set; }
    }
}