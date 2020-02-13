namespace DatingApp.Shared
{
    /// <summary>
    /// A generic error message class.
    /// Used to indicate that the executed request resulted in an unauthorized error.
    /// </summary>
    public class UnauthorizedError : Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedError"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UnauthorizedError(string message) : base(message) { }
    }
}