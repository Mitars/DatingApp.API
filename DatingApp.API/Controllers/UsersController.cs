using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.Models;
using DatingApp.Shared;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Business;
using CSharpFunctionalExtensions;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The users controller class.
    /// </summary>
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="userManager">The user manager.</param>
        public UsersController(IMapper mapper, IUserManager userManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
        }

        /// <summary>
        /// Gets the users for display in a list.
        /// </summary>
        /// <returns>The list of users with limited details.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserForListDto>>> Get([FromQuery]UserParams userParams)
        {
            return await Result.Success<UserParams, Error>(userParams)
                .Tap(u => u.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .Bind(async userParams => {
                    if(string.IsNullOrEmpty(userParams.Gender)) {
                        var user = await this.userManager.Get(userParams.UserId);
                        if (user.IsFailure) {
                            return Result.Failure<UserParams, Error>(user.Error);
                        }

                        userParams.Gender = user.Value.Gender == "male" ? "female" : "male";
                    }
                    
                    return Result.Success<UserParams, Error>(userParams);
                })
                .Bind(this.userManager.GetUsers)
                .Tap(Response.AddPagination)
                .AutoMap(this.mapper.Map<IEnumerable<UserForListDto>>)
                .Finally(result => Ok(result.Value), result => ActionResultError.Get(result.Error, BadRequest));
        }

        /// <summary>
        /// Gets the user details.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The detailed user details.</returns>
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<UserForDetailedDto>> GetUser(int id)
        {
            return await Result.Success<int, Error>(id)
                .Bind(async id => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id ?
                        await this.userManager.GetCurrent(id) :
                        await this.userManager.Get(id))
                .AutoMap(this.mapper.Map<UserForDetailedDto>)
                .Finally(user => Ok(user.Value), result => ActionResultError.Get(result.Error, BadRequest)); 
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="id">The user ID of the user to update.</param>
        /// <param name="userForUpdateDto">The user parameters which should be updated.</param>
        /// <returns>A 204 No Content response if the user has successfully been updated.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            return await Result.Success<User, Error>(this.mapper.Map<UserForUpdateDto, User>(userForUpdateDto))
                .Ensure(u => u.Id == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), ActionResultError.Set(Unauthorized))
                .Tap(this.userManager.Add)
                .Finally(_ => NoContent(), result => ActionResultError.Get(result.Error, BadRequest));            
        }

        /// <summary>
        /// Likes the specified user.
        /// </summary>
        /// <param name="id">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>Returns an OK 200 response if the user has successfully been liked.</returns>
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<ActionResult> LikeUser(int id, int recipientId)
        {
            return await Result.Success<Like, Error>(new Like
                {
                    LikerId = id,
                    LikeeId = recipientId
                })
                .Ensure(l=> l.LikerId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), ActionResultError.Set(Unauthorized))
                .EnsureNull(this.userManager.Get, ActionResultError.Set(() => BadRequest("You already liked this user")))
                .EnsureNotNull(this.userManager.GetByLike, ActionResultError.Set(NotFound))
                .Tap(this.userManager.Add)
                .Bind(this.userManager.Get)
                .Finally(result => Ok(result.Value), result => ActionResultError.Get(result.Error, BadRequest));
        }
    }
}
