namespace DatingApp.API.Models
{
    /// <summary>
    /// The user class.
    /// </summary>
    public class User
    {        
        /// <summary>
        /// Gets and sets the ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets and sets the username.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Gets and sets the password hash.
        /// </summary>
        public byte[] PasswordHash { get; set; }
        
        /// <summary>
        /// Gets and sets the password salt.
        /// </summary>
        public byte[] PasswordSalt { get; set; }        
    }
}