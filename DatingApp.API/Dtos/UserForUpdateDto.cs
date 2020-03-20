namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The updated user data transfer object class.
    /// </summary>
    public class UserForUpdateDto
    {
        /// <summary>
        /// Gets or sets the introduction text.
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// Gets or sets the information for the kind of person the user is looking for.
        /// </summary>
        public string LookingFor { get; set; }

        /// <summary>
        /// Gets or sets the interests.
        /// </summary>
        public string Interests { get; set; }

        /// <summary>
        /// Gets or sets the city in which the user lives.
        /// </summary>
        /// <value></value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the country in which the user lives.
        /// </summary>
        public string Country { get; set; }
    }
}