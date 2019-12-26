using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The authentication controller class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="config">Represents a set of key/value application configuration properties.</param>
        /// <param name="authRepository">The authentication repository.</param>
        private readonly IConfiguration config;
        public AuthController(IConfiguration config, IAuthRepository authRepository)
        {
            this.config = config;
            this.authRepository = authRepository;
        }

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="userForLoginDto">The user to login.</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await this.authRepository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };
         
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

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userForRegisterDto">The user to register.</param>
        /// <returns>The created user.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = new User { Username = userForRegisterDto.Username.ToLower() };

            if (await authRepository.UserExists(userToCreate.Username))
            {
                return BadRequest("Username already exists");
            }

            var createdUser = await authRepository.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);
        }
    }
}
