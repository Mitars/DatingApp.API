using System;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The user authentication attribute.
    /// Used test authentication using the specified user ID parameter and the one within the user claims.
    /// </summary>
    /// <seealso cref="TypeFilterAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserAuthentication : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthentication"/> class.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="realm">The authentication realm.</param>
        public UserAuthentication(string userId, string realm = "Realm")
            : base(typeof(UserAuthenticationFilter))
        {
            Arguments = new object[] { userId, realm };
        }
    }
}
