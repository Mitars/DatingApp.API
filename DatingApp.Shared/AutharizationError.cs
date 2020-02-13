namespace DatingApp.Shared
{
    /// <summary>
    /// The database error class.
    /// Used to indicate that there has been an issue with accessing the database.
    /// </summary>
    public class DatabaseError : Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseError"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public DatabaseError(string message) : base(message) { }
    }
}