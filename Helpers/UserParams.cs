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

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the preferred search gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the search minimum age.
        /// The default is 18 years.
        /// </summary>
        public int MinAge { get; set; } = 18;

        /// <summary>
        /// Gets or sets the search maximum age.
        /// The default is 99 years.
        /// </summary>
        public int MaxAge { get; set; } = 99;

        /// <summary>
        /// Gets or sets the order by information. 
        /// </summary>
        public string OrderBy { get; set; }
    }
}