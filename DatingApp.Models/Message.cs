using System;

namespace DatingApp.Models
{
    /// <summary>
    /// The message class.
    /// </summary>
    public class Message : IBaseEntity
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the sender ID.
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        public virtual User Sender { get; set; }

        /// <summary>
        /// Gets or sets the recipient ID.
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        public virtual User Recipient { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Message"/> has been read.
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

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Message"/> has been deleted on the senders end.
        /// </summary>
        public bool SenderDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Message"/> has been deleted on the recipients end.
        /// </summary>
        public bool RecipientDeleted { get; set; }
    }
}