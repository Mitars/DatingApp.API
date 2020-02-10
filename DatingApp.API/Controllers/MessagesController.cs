using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Business;
using DatingApp.Shared;
using CSharpFunctionalExtensions;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The messages controller class.
    /// </summary>
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMessageManager messageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="messageManager">The message manager.</param>
        /// <param name="mapper">The mapper.</param>
        public MessagesController(IMapper mapper, IMessageManager messageManager)
        {
            this.mapper = mapper;
            this.messageManager = messageManager;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="id">The message ID.</param>
        /// <returns>The message from the user.</returns>
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<ActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = (await this.messageManager.Get(id)).Value;

            if (messageFromRepo == null)
            {
                return NotFound();
            }

            return Ok(messageFromRepo);
        }

        /// <summary>
        /// Gets all the messages from the <see cref="User"/> with the specified <see cref="MessageParams"/>.
        /// Supports pagination.
        /// </summary>
        /// <param name="userId">The user ID from which the messages will be retrived.</param>
        /// <param name="messageParams">The message parameters used for filtering out the messages.</param>
        /// <returns>The list of messages</returns>
        [HttpGet]
        public async Task<ActionResult> GetMessages(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            return await this.messageManager.Get(messageParams)
                .Tap(Response.AddPagination)
                .AutoMap(this.mapper.Map<IEnumerable<MessageToReturnDto>>)
                .Finally(result => Ok(result), result => ActionResultError.Get(result.Error, BadRequest));
        }

        /// <summary>
        /// Gets the message thread between the current and another <see cref="User"/>.
        /// </summary>
        /// <param name="userId">The current user ID.</param>
        /// <param name="recipientId">The recipient user ID.</param>
        /// <returns>The conversation thread between the two users.</returns>
        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IEnumerable<MessageToReturnDto>>> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            return await this.messageManager.GetThread(userId, recipientId)
                .AutoMap(this.mapper.Map<IEnumerable<MessageToReturnDto>>)
                .Finally(result => Ok(result), result => ActionResultError.Get(result.Error, BadRequest));
        }

        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="userId">The user who created the message.</param>
        /// <param name="messageForCreationDto">The created message.</param>
        /// <returns>The created at root response with the details of the created message.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = this.mapper.Map<Message>(messageForCreationDto);

            return await this.messageManager.Add(userId, message)
                .Finally(result => CreatedAtRoute("GetMessage", new { userId, id = result.Value.Id }, result.Value), result => ActionResultError.Get(result.Error, BadRequest));
        }

        /// <summary>
        /// Sets the <see cref="Message"/> as deleted.
        /// Only deletes the message if both users set the message as deleted.
        /// </summary>
        /// <param name="id">The message ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>Returns an OK 200 response if the message has successfully been deleted.</returns>
        [HttpPost("{id}")]
        public async Task<ActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            return await this.messageManager.Delete(userId, id)
                .Finally(result => NoContent(), result => ActionResultError.Get(result.Error, BadRequest));
        }

        /// <summary>
        /// Marks the message as read.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="id">The message ID.</param>
        /// <returns>A 204 No Content response if the message has successfully been marked as read.</returns>
        [HttpPost("{id}/read")]
        public async Task<ActionResult> MarkMessageAsRead(int userId, int id) =>
            await this.messageManager.MarkAsRead(userId, id)
                .Finally(result => NoContent(), result => ActionResultError.Get(result.Error, BadRequest));      
    }
}