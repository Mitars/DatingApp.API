using DatingApp.Shared.ErrorTypes;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The action result error class.
    /// Used to indicate that the executed request resulted in an action result.
    /// </summary>
    public class ActionResultError : Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResultError"/> class.
        /// </summary>
        /// <param name="result">The action function that returns an action result.</param>
        private ActionResultError(Func<ActionResult> result)
            : base(string.Empty) =>
            this.Result = result;

        /// <summary>
        /// Gets or sets the result function.
        /// </summary>
        public Func<ActionResult> Result { get; private set; }

        /// <summary>
        /// Gets the action result error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="func">The action result wrapper used if it is a non <see cref="ActionResultError"/> type.</param>
        /// <returns>The action result with the containing error.</returns>
        public static ActionResult Get(Error error, Func<string, ActionResult> func) =>
            (error as ActionResultError)?.Result() ?? func(error.Message);
    }
}