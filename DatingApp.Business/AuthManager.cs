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
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
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
        public virtual async Task<Result<User, Error>> Login(UserForLoginDto userForLoginDto) =>
            await this.userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u =>  u.NormalizedUserName == userForLoginDto.Username.ToUpper())
                .Success()
                .Ensure(async u => (await this.signInManager.CheckPasswordSignInAsync(u, userForLoginDto.Password, false)).Succeeded, new UnauthorizedError("Unauthorized"));

        /// <inheritdoc />
        public async Task<Result<User, Error>> Register(UserForRegisterDto userForRegisterDto) =>
            await userForRegisterDto.Success()
                .AutoMap(this.mapper.Map<User>)
                .Ensure(async u => (await this.userManager.CreateAsync(u, userForRegisterDto.Password)).Succeeded, new Error("Failed creating the user"))
                .Tap(async u => await this.userManager.AddToRoleAsync(u, "Member"));
        
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

            var tokenHandler = new JwtSecurityTokenHandler();            
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            .Success()
            .Bind(symmetricSecurityKey => new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature).Success())
            .Bind(credentials => new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = credentials
                }.Success())
            .Bind(tokenDescriptor => tokenHandler.CreateToken(tokenDescriptor).Success())
            .Bind(token => tokenHandler.WriteToken(token).Success());
        }
    }
}