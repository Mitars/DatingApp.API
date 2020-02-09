using System;
using DatingApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Helpers
{
    public class ActionResultError : Error
{
    private ActionResultError(Func<ActionResult> result)
        : base(string.Empty)
    {
        this.Result = result;
    }

    public Func<ActionResult> Result { get; private set; }

    public static ActionResult Get(Error e, Func<string, ActionResult> f)
    {
        return (e as ActionResultError)?.Result() ?? f(e.Message);
    }

    public static Error Set(Func<ActionResult> a)
    {
        return new ActionResultError(a);
    }
}

}