using System;
using System.Threading;

namespace Aleph1.Logging.Tester
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			GoodFunction("avraham", 5);
			try
			{
				BadFunction("avraham", 5);
			}
			catch (Exception) { }
		}

		[Logged(LogReturnValue = true)]
		private static string GoodFunction(string name, int age)
		{
			Thread.Sleep(2450);
			return $"{name} is: {age} years old, yeah.";
		}

		[Logged]
		private static string BadFunction(string name, int age)
		{
			throw new NotImplementedException();
		}
	}
}
