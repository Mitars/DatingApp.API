using System;
using DatingApp.Shared.ErrorTypes;
using Microsoft.AspNetCore.Mvc;

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
        : base(string.Empty)
    {
        this.Result = result;
    }

    /// <summary>
    /// Gets or sets the result function.
    /// </summary>
    public Func<ActionResult> Result { get; private set; }
    
    public static ActionResult Get(Error error, Func<string, ActionResult> func)
    {
        return (error as ActionResultError)?.Result() ?? func(error.Message);
    }

    public static Error Set(Func<ActionResult> func)
    {
        return new ActionResultError(func);
    }
}

}