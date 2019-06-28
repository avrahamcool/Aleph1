using Aleph1.Utitilies;
using NLog;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aleph1.Logging
{
    /// <summary>Helper for NLog Logger</summary>
    public static class LoggerHelper
    {
        /// <summary>Log a message in the Aleph1 format</summary>
        /// <param name="logger">the logger to lof to</param>
        /// <param name="logLevel">log level</param>
        /// <param name="message">message</param>
        /// <param name="exception">optional - exception</param>
        /// <param name="correlationID">optional - can be used to identify multiple messages as part of an action</param>
        /// <param name="className">optional - caller class name - calculated at runtime</param>
        /// <param name="methodName">optional - caller method name - calculated at buildtime</param>
        public static void LogAleph1(this ILogger logger, LogLevel logLevel, string message, Exception exception = null, object correlationID = null, string className = null, [CallerMemberName] string methodName = "")
        {
            LogEventInfo lei = new LogEventInfo(logLevel, logger.Name, message);
            
            className = className ?? new StackFrame(1, false).GetMethod().DeclaringType.Name;
            
            lei.Properties.Add("A1_UserName", UserExtentions.CurrentUserName);
            lei.Properties.Add("A1_ClassName", className);
            lei.Properties.Add("A1_MethodName", methodName);
            lei.Properties.Add("A1_CorrelationID", correlationID);

            lei.Exception = exception;

            logger.Log(lei);
        }
    }
}
