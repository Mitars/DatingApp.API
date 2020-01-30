using System;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The message to return data transfer object.
    /// </summary>
    public class MessageToReturnDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the sender ID.
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the sender known as name.
        /// </summary>
        public string SenderKnownAs { get; set; }

        /// <summary>
        /// Gets or sets the sender photo URL.
        /// </summary>
        public string SenderPhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the recipient ID.
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the recipient known as name.
        /// </summary>
        public string RecipientKnownAs { get; set; }

        /// <summary>
        /// Gets or sets the recipient photo URL.
        /// </summary>
        public string RecipientPhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this message has been read.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets the time when the recipient has read the message.
        /// </summary>
        public DateTime? DateRead { get; set; }

        /// <summary>
        /// Gets or sets the time when the message was sent.
        /// </summary>
        public DateTime MessageSent { get; set; }
    }
}