using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The fallback class.
    /// Used to enable angular routing.
    /// </summary>
    [AllowAnonymous]
    public class Fallback : Controller
    {
        /// <summary>
        /// The fallback method which retrives the index.
        /// </summary>
        /// <returns>The index file.</returns>
        public ActionResult Index()
        {
            return PhysicalFile(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "index.html"),
                "text/HTML");
        }
    }
}