using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.API.Helpers
{

    /// <summary>
    /// The user authentication filter.
    /// Confirms the user has rights to make the request by comparing the specified user ID parameter and the one within the user claims.
    /// </summary>
    /// <seealso cref="IAuthorizationFilter" />
    public class UserAuthenticationFilter : IAuthorizationFilter
    {
        private readonly string userId;
        private readonly string realm;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticationFilter"/> class.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="realm">The authentication realm.</param>
        public UserAuthenticationFilter(string userId, string realm)
        {
            this.userId = userId;
            this.realm = realm;

            if (string.IsNullOrWhiteSpace(this.realm))
            {
                throw new ArgumentNullException(nameof(this.realm), @"Please provide a non-empty realm value.");
            }
        }

        /// <inheritdoc />
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var currentUserId = int.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (!context.RouteData.Values.TryGetValue(this.userId, out var value))
                {
                    return;
                }

                var requestUserId = Convert.ToInt32(value);
                if (currentUserId != requestUserId)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{realm}\"";
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
