using System;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The message for creation data transfer object.
    /// </summary>
    public class MessageForCreationDto
    {
        /// <summary>
        /// Gets or sets the sender ID.
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the recipient ID.
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the time when the message was sent.
        /// </summary>
        public DateTime MessageSent { get; set; } = DateTime.Now;
    }
}