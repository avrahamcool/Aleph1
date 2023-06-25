using System;

namespace Aleph1.Logging.Tester
{
	internal class Program
	{
		[Logged]
		private static void Main(string[] args)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

			SomeFunction("I'm happy", false, 50);
			SomeFunction("I'm sad", true, 50);
			SomeFunction("I'm happy again - will never reach", false, 50);
		}

		[Logged(LogReturnValue = true)]
		private static string SomeFunction(string someString, bool shouldThrow, int delay)
		{
			if (shouldThrow)
			{
				throw new NotImplementedException("Look at inner", new Exception("surprise"));
			}

			System.Threading.Thread.Sleep(delay);
			return someString;
		}
	}
}
