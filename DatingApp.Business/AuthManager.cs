using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Business
{
    /// <summary>
    /// The auth manager class.
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthManager"/> class.
        /// </summary>
        public AuthManager(
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        
        /// <inheritdoc />
        public virtual async Task<Result<User, Error>> Login(UserForLoginDto userForLoginDto)
        {
            var user = await this.userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u =>  u.NormalizedUserName == userForLoginDto.Username.ToUpper());
            var result = await this.signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (!result.Succeeded)
            {
                return Result.Failure<User, Error>(new UnauthorizedError("Unauthorized"));                
            }

            return user.Success();
        }

        /// <inheritdoc />
        public async Task<Result<User, Error>> Register(UserForRegisterDto userForRegisterDto)
        {
            var user = this.mapper.Map<User>(userForRegisterDto);

            var result = await this.userManager.CreateAsync(user, userForRegisterDto.Password);

            if (!result.Succeeded)
            {
                return Result.Failure<User, Error>(new Error("Failed creating the user"));                
            }

            await this.userManager.AddToRoleAsync(user, "Member");

            return user.Success();
        }
        
        /// <inheritdoc />
        public async Task<Result<string, Error>> GenerateJwt(User user, string key)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await this.userManager.GetRolesAsync(user);
            roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)));

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            
            return result.Success();
        }
    }
}