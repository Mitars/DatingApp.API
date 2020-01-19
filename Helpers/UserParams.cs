using System;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The user params class.
    /// Sent along with the user request.
    /// </summary>
    public class UserParams
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
    }
}