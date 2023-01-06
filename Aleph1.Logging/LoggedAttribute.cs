using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

using NLog;

using PostSharp.Aspects;

namespace Aleph1.Logging
{
	/// <summary>Aspect to handle logging</summary>
	[Serializable, AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true), LinesOfCodeAvoided(20)]
	public sealed class LoggedAttribute : OnMethodBoundaryAspect
	{
		/// <summary>Default = true, set to False when you don't want the parameters of the function to be logged</summary>
		public bool LogParameters { get; set; } = true;

		/// <summary>Default = false, set to true when you want the return value of the function to be logged</summary>
		public bool LogReturnValue { get; set; } = false;

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
			logger = LogManager.GetLogger(ClassName);
		}

		/// <summary>Handle the logging of entering a function. depends on LogParameters</summary>
		/// <param name="args"></param>
		public sealed override void OnEntry(MethodExecutionArgs args)
		{
			args.MethodExecutionTag = Guid.NewGuid();
			string message = LogParameters ? $"Entering with parameters: {GetArguments(args)}" : "Entering";
			logger.LogAleph1(LogLevel.Trace, message, null, args.MethodExecutionTag, ClassName, MethodName);
		}

		/// <summary>Handle the logging of exiting a function. depends on LogReturnValue</summary>
		/// <param name="args"></param>
		public sealed override void OnExit(MethodExecutionArgs args)
		{
			string message = LogReturnValue ? $"Leaving with result: {GetReturnValue(args)}" : "Leaving";
			logger.LogAleph1(LogLevel.Trace, message, null, args.MethodExecutionTag, ClassName, MethodName);
		}

		/// <summary>Handle the logging of an error in a function</summary>
		/// <param name="args"></param>
		public sealed override void OnException(MethodExecutionArgs args)
		{
			logger.LogAleph1(LogLevel.Error, $"parameters: {GetArguments(args)}", args.Exception, args.MethodExecutionTag, ClassName, MethodName);
		}

		private string GetArguments(MethodExecutionArgs args)
		{
			if (ParameterNames.Length == 0)
			{
				return "null";
			}

			Dictionary<string, object> o = new Dictionary<string, object>();
			for (int i = 0; i < ParameterNames.Length; i++)
			{
				o.Add(ParameterNames[i], args.Arguments[i]);
			}

			try { return JsonConvert.SerializeObject(o); }
			catch (JsonSerializationException e) { return $"[Error in Serializing the arguments: {e.Message}]"; }
		}
		private static string GetReturnValue(MethodExecutionArgs args)
		{
			if (args.ReturnValue == null)
			{
				return "null";
			}

			try { return JsonConvert.SerializeObject(args.ReturnValue); }
			catch (JsonSerializationException e) { return $"[Error in Serializing the return value: {e.Message} ---ReturnValue.ToString: {args.ReturnValue}]"; }
		}
	}
}
