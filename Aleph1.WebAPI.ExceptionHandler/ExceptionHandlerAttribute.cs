using System;
using System.Web.Http.Filters;

namespace Aleph1.WebAPI.ExceptionHandler
{
    /// <summary>Let the Actions use a frendly message</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        /// <summary>The message to show for the client</summary>
        public string CustomMessage { get; set; }

        /// <summary>You have to specify a Custom message</summary>
        /// <param name="CustomMessage">The message to show for the client</param>
        public ExceptionHandlerAttribute(string CustomMessage)
        {
            this.CustomMessage = CustomMessage;
        }


        /// <summary>replace the current Exception with a new one</summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Exception = new Exception(CustomMessage, actionExecutedContext.Exception);
        }
    }
}
