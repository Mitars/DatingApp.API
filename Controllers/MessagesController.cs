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
    /// The messages controller class.
    /// </summary>
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [Authorize]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="repo">The dating repository.</param>
        /// <param name="mapper">The mapper.</param>
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this.mapper = mapper;
            this.repo = repo;
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

            var messageFromRepo = await this.repo.GetMessage(id);

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

            var messagesFromRepo = await this.repo.GetMessagesForUser(messageParams);

            var messagesToReturn = this.mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(
                messagesFromRepo.CurrentPage,
                messagesFromRepo.ItemsPerPage,
                messagesFromRepo.TotalItems,
                messagesFromRepo.TotalPages);

            return Ok(messagesToReturn);
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

            var messageFromRepo = await this.repo.GetMessageThread(userId, recipientId);

            var messageThread = this.mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            return Ok(messageThread);
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
            var sender = await this.repo.GetUser(userId);
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            if (userId == messageForCreationDto.RecipientId)
            {
                return BadRequest("Cannot send message to self");
            }

            messageForCreationDto.SenderId = sender.Id;
            var recipient = await this.repo.GetUser(messageForCreationDto.RecipientId);
            if (recipient == null)
            {
                return BadRequest("Could not find user");
            }

            var message = this.mapper.Map<Message>(messageForCreationDto);

            this.repo.Add(message);

            if (await this.repo.SaveAll())
            {
                var messageToReturn = this.mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }

            throw new Exception("Creating the message failed on save");
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

            var messageFromRepo = await this.repo.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }
            else if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                this.repo.Delete(messageFromRepo);
            }

            if (await this.repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error deleting the message");
        }

        /// <summary>
        /// Marks the message as read.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="id">The message ID.</param>
        /// <returns>A 204 No Content response if the message has successfully been marked as read.</returns>
        [HttpPost("{id}/read")]
        public async Task<ActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = await this.repo.GetMessage(id);

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await this.repo.SaveAll();

            return NoContent();
        }
    }
}