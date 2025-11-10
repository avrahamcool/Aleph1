using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Aleph1.Utilities;

using NLog;

namespace Aleph1.Logging
{
	/// <summary>Helper for NLog Logger</summary>
	public static class LoggerHelper
	{
		/// <summary>Log a message in the Aleph1 format</summary>
		/// <param name="logger">the logger to log to</param>
		/// <param name="logLevel">log level</param>
		/// <param name="message">message</param>
		/// <param name="exception">exception</param>
		/// <param name="parameters">parameters</param>
		/// <param name="returnValue">returnValue</param>
		/// <param name="elapsedMilliseconds">function execution time - calculated at runtime</param>
		/// <param name="className">caller class name - calculated at runtime</param>
		/// <param name="methodName">caller method name - calculated at build time</param>
#if NET40
		public static void LogAleph1(this ILogger logger, LogLevel logLevel, string message,
			Exception exception = null, long elapsedMilliseconds = 0,
			string parameters = null, string returnValue = null,
			string className = null, string methodName = "")
#else
		public static void LogAleph1(this ILogger logger, LogLevel logLevel, string message,
			Exception exception = null, long elapsedMilliseconds = 0,
			string parameters = null, string returnValue = null,
			string className = null, [CallerMemberName] string methodName = "")
#endif
		{
			if (!logger.IsEnabled(logLevel))
			{
				return;
			}

			LogEventInfo lei = new LogEventInfo(logLevel, logger.Name, message);

			lei.Properties.Add("A1_UserName", UserExtentions.CurrentUserName);

			lei.Properties.Add("A1_ClassName", className ?? new StackFrame(1, false).GetMethod().DeclaringType.Name);
			lei.Properties.Add("A1_MethodName", methodName);

			lei.Properties.Add("A1_ElapsedMilliseconds", elapsedMilliseconds);
			lei.Properties.Add("A1_Parameters", parameters);
			lei.Properties.Add("A1_ReturnValue", returnValue);
			lei.Properties.Add("A1_Exception", exception?.ToString());

			lei.Exception = exception;

			logger.Log(lei);
		}
	}
}
