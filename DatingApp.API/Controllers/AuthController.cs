using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DatingApp.Business.Dtos;
using DatingApp.Business;
using DatingApp.API.Helpers;
using DatingApp.Shared.FunctionalExtensions;
using CSharpFunctionalExtensions;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The authentication controller class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IConfiguration config;
        private readonly IAuthManager authManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="config">Represents a set of key/value application configuration properties.</param>
        /// <param name="authManager">The auth manager.</param>
        /// <param name="mapper">The mapper.</param>
        public AuthController(
            IMapper mapper,
            IConfiguration config,
            IAuthManager authManager)
        {
            this.mapper = mapper;
            this.config = config;
            this.authManager = authManager;
        }

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="userForLoginDto">The user to login.</param>
        /// <returns>The logged in user.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserForLoginDto userForLoginDto)
        {
            return await this.authManager.Login(userForLoginDto)
                .Finally(
                    u => Ok(new {
                        token = (this.authManager.GenerateJwt(u.Value, this.config.GetSection("AppSettings:Token").Value)).Result.Value,
                        user = this.mapper.Map<User, UserForListDto>(u.Value)}),
                    u => ActionResultError.Get(u.Error, BadRequest));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userForRegisterDto">The user to register.</param>
        /// <returns>The created user.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserForRegisterDto userForRegisterDto) =>
            await this.authManager.Register(userForRegisterDto)
                .Finally(
                    u => CreatedAtRoute("GetUser", new { controller = "Users", id = u.Value.Id }, u.Value),
                    u => ActionResultError.Get(u.Error, BadRequest));

    }
}
