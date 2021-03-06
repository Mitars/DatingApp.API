namespace DatingApp.Models
{
    /// <summary>
    /// The like class.
    /// </summary>
    public class Like
    {
        /// <summary>
        /// Gets or sets the ID of the user that liked another user.
        /// </summary>
        public int LikerId { get; set; }

        /// <summary>
        /// Gets or sets the user that liked another user.
        /// </summary>
        public virtual User Liker { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user that has been liked.
        /// </summary>
        public int LikeeId { get; set; }

        /// <summary>
        /// Gets or sets the user that has been liked.
        /// </summary>
        public virtual User Likee { get; set; }
    }
}