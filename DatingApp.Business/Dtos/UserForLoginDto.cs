namespace DatingApp.Business.Dtos
{
    /// <summary>
    /// The class user used for registration.
    /// </summary>
    public class UserForLoginDto
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}