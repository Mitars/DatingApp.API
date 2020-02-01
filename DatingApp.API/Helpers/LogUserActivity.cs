using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.DataAccess;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

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
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetCurrentUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}