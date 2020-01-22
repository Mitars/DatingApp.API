using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The users controller class.
    /// </summary>
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="repo">The dating repository.</param>
        /// <param name="mapper">The mapper.</param>
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this.mapper = mapper;
            this.repo = repo;
        }

        /// <summary>
        /// Gets the users for display in a list.
        /// </summary>
        /// <returns>The list of users with limited details.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserForListDto>>> Get([FromQuery]UserParams userParams)
        {            
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                var userFromRepo = await this.repo.GetUser(currentUserId);
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await this.repo.GetUsers(userParams);

            var usersToReturn = this.mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.ItemsPerPage, users.TotalItems, users.TotalPages);

            return Ok(usersToReturn);
        }

        /// <summary>
        /// Gets the user details.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The detailed user details.</returns>
        [HttpGet("{id}", Name="GetUser")]
        public async Task<ActionResult<UserForDetailedDto>> GetUser(int id)
        {
            var user = await this.repo.GetUser(id);

            var userToReturn = this.mapper.Map<UserForDetailedDto>(user);

            return userToReturn;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="id">The user ID of the user to update.</param>
        /// <param name="userForUpdateDto">The user parameters which should be updated.</param>
        /// <returns>A 204 No Content response if the user has successfully been updated.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await this.repo.GetUser(id);
            
            this.mapper.Map(userForUpdateDto, userFromRepo);

            if (await this.repo.SaveAll()) {
                return NoContent();
            }

            throw new Exception($"Updating user {id} failed on save");
        }

        /// <summary>
        /// Likes the specified user.
        /// </summary>
        /// <param name="id">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>Returns an OK 200 response if the user has successfully been liked.</returns>
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<ActionResult<Like>> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            if (await this.repo.GetLike(id, recipientId) != null)
            {
                return BadRequest("You already like this user");
            }

            if (await this.repo.GetUser(recipientId) == null)
            {
                return NotFound();
            }

            var like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            this.repo.Add<Like>(like);

            if (await this.repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        } 
    }
}