using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
        private readonly IConfiguration config;
        private readonly IMapper mapper;
        private readonly IDatingRepository repository;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="config">Represents a set of key/value application configuration properties.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="repository">The dating repository.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        public AuthController(
            IConfiguration config,
            IMapper mapper,
            IDatingRepository repository,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.config = config;
        }

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="userForLoginDto">The user to login.</param>
        /// <returns>The logged in user.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserForLoginDto userForLoginDto)
        {
            var user = await this.userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u =>  u.NormalizedUserName == userForLoginDto.Username.ToUpper());
            var result = await this.signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var userForListDto = this.mapper.Map<UserForListDto>(user);
            return Ok(new { token = this.GenerateJwt(user).Result, user = userForListDto });
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userForRegisterDto">The user to register.</param>
        /// <returns>The created user.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = this.mapper.Map<User>(userForRegisterDto);

            var result = await this.userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userToReturn = this.mapper.Map<UserForDetailedDto>(userToCreate);

            return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
        }

        /// <summary>
        /// Generates the JSON Web Token using the <see cref="User"/>.
        /// </summary>
        /// <param name="user">The user used for generating the JWT.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The generated JWT.
        /// </returns>
        private async Task<string> GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await this.userManager.GetRolesAsync(user);
            roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
