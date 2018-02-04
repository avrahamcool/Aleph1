using System;
using System.Web.Http.Filters;

namespace Aleph1.WebAPI.ExceptionHandler
{
    /// <summary>Let the Actions use a frendly message</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FriendlyMessageAttribute : ExceptionFilterAttribute
    {
        /// <summary>The message to show for the client</summary>
        public string FriendlyMessage { get; set; }

        /// <summary>You have to specify a Friendly message</summary>
        /// <param name="friendlyMessage">The message to show for the client</param>
        public FriendlyMessageAttribute(string friendlyMessage)
        {
            this.FriendlyMessage = friendlyMessage;
        }

        /// <summary>replace the current Exception with a new one</summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Exception = new Exception(FriendlyMessage, actionExecutedContext.Exception);
        }
    }
}
