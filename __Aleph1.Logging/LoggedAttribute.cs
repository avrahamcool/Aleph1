using Aleph1.Utitilies;
using NLog;
using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Aleph1.Logging
{
    /// <summary>Aspect to hanle logging</summary>
    [Serializable, AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class LoggedAttribute : OnMethodBoundaryAspect
    {
        /// <summary>Default = true, set to False when you dont want the parameters of the function to be logged</summary>
        public bool LogParameters { get; set; } = true;

        /// <summary>Default = false, set to true when you want the return value of the function to be logged</summary>
        public bool LogReturnValue { get; set; } = false;

        [NonSerialized]
        private JavaScriptSerializer serializer;

        [NonSerialized]
        private ILogger logger;

        private string[] ParameterNames { get; set; }
        private string ClassName { get; set; }
        private string MethodName { get; set; }

        /// <summary>Initializing the fixed fields at compile time to improve performance</summary>
        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            ClassName = method.ReflectedType.Name;
            MethodName = method.Name;
            ParameterNames = method.GetParameters().Select(pi => pi.Name).ToArray();
        }

        /// <summary>Initializing the run time fields</summary>
        public override void RuntimeInitialize(MethodBase method)
        {
            logger = LogManager.GetCurrentClassLogger();
            if (LogParameters || LogReturnValue)
                serializer = new JavaScriptSerializer();
        }

        /// <summary>Hanlde the logging of entering a function. depends on LogParameters</summary>
        /// <param name="args"></param>
        public override sealed void OnEntry(MethodExecutionArgs args)
        {
            args.MethodExecutionTag = Guid.NewGuid();
            string message = LogParameters ? $"Entering with parameters: {GetArguments(args)}" : "Entering";
            logger.Log(CreateEventInfo(LogLevel.Trace, message, args.MethodExecutionTag));
        }

        /// <summary>Hanlde the logging of exiting a function. depends on LogReturnValue</summary>
        /// <param name="args"></param>
        public override sealed void OnExit(MethodExecutionArgs args)
        {
            string message = LogReturnValue ? $"Leaving with result: {GetReturnValue(args)}" : "Leaving";
            logger.Log(CreateEventInfo(LogLevel.Trace, message, args.MethodExecutionTag));
        }

        /// <summary>Hanlde the logging of an error in a function</summary>
        /// <param name="args"></param>
        public override sealed void OnException(MethodExecutionArgs args)
        {
            LogEventInfo lei = CreateEventInfo(LogLevel.Error, args.Exception.StackTrace, args.MethodExecutionTag);
            lei.Exception = args.Exception;

            logger.Log(lei);
        }


        private LogEventInfo CreateEventInfo(LogLevel logLevel, string message, object correlationID)
        {
            LogEventInfo retVal = new LogEventInfo(logLevel, null, message);

            retVal.Properties.Add("A1_UserName", UserExtentions.CurrentUserName);
            retVal.Properties.Add("A1_ClassName", ClassName);
            retVal.Properties.Add("A1_MethodName", MethodName);
            retVal.Properties.Add("A1_CorrelationID", correlationID);

            return retVal;
        }
        private string GetArguments(MethodExecutionArgs args)
        {
            if (ParameterNames.Length == 0)
                return "null";

            Dictionary<string, object> o = new Dictionary<string, object>();
            for (int i = 0; i < ParameterNames.Length; i++)
                o.Add(ParameterNames[i], args.Arguments[i]);
            try { return serializer.Serialize(o); }
            catch (Exception e) { return $"[Error in Serializing the arguments: {e.Message}]"; }
        }
        private string GetReturnValue(MethodExecutionArgs args)
        {
            if (args.ReturnValue == null)
                return "null";

            try { return serializer.Serialize(args.ReturnValue); }
            catch (Exception e) { return $"[Error in Serializing the return value: {e.Message} ---ReturnValue.ToString: {args.ReturnValue.ToString()}]"; }
        }
    }
}
