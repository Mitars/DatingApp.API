namespace DatingApp.Shared
{
    /// <summary>
    /// A generic error message class.
    /// Used as the parent for all error messages when used alongside the <see cref="FunctionalExtensions"/>
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