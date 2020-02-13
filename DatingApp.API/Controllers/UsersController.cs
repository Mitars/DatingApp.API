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
            return await userParams.Success()
                .Tap(u => u.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .Bind(this.userManager.Get)                
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
            return await id.Success()
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
            return await userForUpdateDto.Success()
                .AutoMap(this.mapper.Map<UserForUpdateDto, User>)
                .Ensure(u => id == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), new UnauthorizedError("Cannot update other users"))
                .Bind(this.userManager.Update)
                .Finally(u => NoContent(), result => ActionResultError.Get(result.Error, BadRequest));            
        }

        /// <summary>
        /// Likes the specified user.
        /// </summary>
        /// <param name="id">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>Returns an OK 200 response if the user has successfully been liked.</returns>
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<ActionResult> LikeUser(int id, int recipientId) =>
            await this.userManager.AddLike(id, recipientId)
                .Ensure((Like like) => id == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), new UnauthorizedError("Cannot like as another user"))                
                .Finally(result => Ok(result.Value), result => ActionResultError.Get(result.Error, BadRequest));
    }
}
