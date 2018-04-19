using System;

namespace DrEAM.errors
{

	/// <summary>
	/// Created by AndreiM on 12/27/2016.
	/// </summary>
	public class JuliarLogger
	{
		private static bool hasError = false;
		private static int errors = 0;


		public static bool hasErrors()
		{
			return hasError;
		}

		public static void log(string msg)
		{
			Console.WriteLine(msg);
		}

		public static void log(string msg, Exception e)
		{
			Console.WriteLine(msg);
		}

		public static void log(Exception e)
		{
			e.printStackTrace(err);
		}


		public static void logerr(string msg)
		{
			Console.Error.WriteLine(msg);
		}

		public static void errorFound()
		{
			hasError = true;
			errors++;
		}

		public static int NumberOfErrors
		{
			get
			{
				return errors;
			}
		}

		public static void exitIfErrors()
		{
			if (JuliarLogger.hasError)
			{
				Console.Error.WriteLine("Found " + errors + " errors!");
				throw new System.NullReferenceException();
			}
		}

		public JuliarLogger(string Message)
		{
			Console.WriteLine("Error: " + Message);
		}

		public JuliarLogger(string Message, Exception Type)
		{
			Console.WriteLine("Error: " + Message);
		}

		public static void message(string message)
		{
			Console.WriteLine(message);
		}
	}
}