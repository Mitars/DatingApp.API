using DatingApp.Business;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The log user activity class.
    /// </summary>
    public class LogUserActivity : IAsyncActionFilter
    {
        /// <summary>
        /// Updates the user activity whenever a service filter is invoked.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext.</param>
        /// <param name="next">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate.
        /// Invoked to execute the next action filter or the action itself.</param>
        /// <returns>A System.Threading.Tasks.Task that on completion indicates the filter has executed.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userManager = resultContext.HttpContext.RequestServices.GetService<IUserManager>();
            await userManager.UpdateActivity(userId);
        }
    }
}