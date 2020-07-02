using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.Business;
using DatingApp.Models;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

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
        /// <param name="userParams">The search parameters of the user.</param>
        /// <returns>The list of users with limited details.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserForListDto>>> Get([FromQuery] UserParams userParams) =>
            await userParams.Success()
                .TapIf(userParams => userParams.UserId == 0, userParams => userParams.UserId = this.GetCurrentUserId())
                .Bind(this.userManager.Get)
                .Tap(Response.AddPagination)
                .Bind(pagedList => this.mapper.Map<PagedList<User>, IEnumerable<UserForListDto>>(pagedList, opt => opt.AfterMap((src, dest) => AutoMapperProfiles.UpdateIsLiked(src, dest, userParams.UserId))))
                .Finally(result => Ok(result), error => ActionResultError.Get(error, BadRequest));

        /// <summary>
        /// Gets the user details.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The detailed user details.</returns>
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<UserForDetailedDto>> GetUser(int id) =>
            await id.Success()
                .Bind(async id => this.IsAuthenticated(id) ?
                        await this.userManager.GetCurrent(id) :
                        await this.userManager.Get(id))
                .Bind(this.mapper.Map<UserForDetailedDto>)
                .Finally(result => Ok(result), error => ActionResultError.Get(error, BadRequest));

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="id">The user ID of the user to update.</param>
        /// <param name="userForUpdateDto">The user params used for filtering the user list.</param>
        /// <returns>A 204 No Content response if the user has successfully been updated.</returns>
        [HttpPut("{id}")]
        [UserAuthentication("id")]
        public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) =>
            await id.Success()
                .Bind(this.userManager.Get)
                .Bind(user => this.mapper.Map(userForUpdateDto, user))
                .Tap(user => user.Id = id)
                .Bind(this.userManager.Update)
                .Finally(_ => NoContent(), error => ActionResultError.Get(error, BadRequest));

        /// <summary>
        /// Likes the specified user.
        /// </summary>
        /// <param name="id">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>Returns an OK 200 response if the user has successfully been liked.</returns>
        [HttpPost("{id}/like/{recipientId}")]
        [UserAuthentication("id")]
        public async Task<ActionResult> LikeUser(int id, int recipientId) =>
            await (userId: id, recipientId).Success()
                .Bind(ids => this.userManager.AddLike(ids.userId, ids.recipientId))
                .Finally(_ => Ok(), error => ActionResultError.Get(error, BadRequest));

        /// <summary>
        /// Deletes the like between the users.
        /// </summary>
        /// <param name="id">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>Returns an OK 200 response if the user has successfully been liked.</returns>
        [HttpDelete("{id}/like/{recipientId}")]
        [UserAuthentication("id")]
        public async Task<ActionResult> UnlikeUser(int id, int recipientId) =>
            await (userId: id, recipientId).Success()
                .Bind(ids => this.userManager.DeleteLike(ids.userId, ids.recipientId))
                .Finally(_ => Ok(), error => ActionResultError.Get(error, BadRequest));
    }
}
