namespace DatingApp.Shared.ErrorTypes
{
    /// <summary>
    /// A generic error message class.
    /// Used as the parent for all error messages when used alongside the <see cref="FunctionalExtensions"/>.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public Error(string message) =>
            this.Message = message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; }
    }
}