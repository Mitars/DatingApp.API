namespace DatingApp.Shared.ErrorTypes
{
    /// <summary>
    /// The not found error class.
    /// Used to indicate that the resource has not been found.
    /// </summary>
    public class NotFoundError : Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundError"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NotFoundError(string message)
            : base(message) { }
    }
}