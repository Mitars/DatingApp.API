using System;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The message params class.
    /// Sent along with the message request.
    /// </summary>
    public class MessageParams
    {
        private const int MaxPageSize = 50;
        private int pageSize = 10;

        /// <summary>
        /// Gets or sets the page number which to retrieve.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size.
        /// The maximum value is 10 entries per page.
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = Math.Min(value, MaxPageSize); }
        }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the messages container.
        /// </summary>
        public string MessageContainer { get; set; } = "Unread";
    }
}