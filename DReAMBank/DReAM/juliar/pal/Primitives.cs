using System;
using System.Collections.Generic;
using System.Text;

namespace DrEAM.pal
{

	using JuliarLogger = com.juliar.errors.JuliarLogger;

	/// <summary>
	/// platform abstraction layer.
	/// </summary>
	public class Primitives
	{
		public static string sysFileOpen(string path)
		{
			path = stripQuotes(path);
			try
			{
				int read = 1024;
				int N = 1024 * read;
				char[] buffer = new char[N];
				StringBuilder text = new StringBuilder();

				FileReader reader = new FileReader(path);
				BufferedReader bufferedReader = new BufferedReader(reader);

				while (true)
				{
					read = bufferedReader.read(buffer, 0, N);
					text.Append(new string(buffer, 0, read));

					if (read < N)
					{
						break;
					}
				}

				return text.ToString();

			}
			catch (Exception fne)
			{
				JuliarLogger.log(fne.Message);
			}

			return "";
		}

		public static string sysExec(string execString)
		{
			string command = "/bin/sh";
			string param = "-c";
			if (System.getProperty("os.name").ToLower().contains("win"))
			{
				command = "cmd.exe";
				param = "/C";
			}
			ProcessBuilder builder = new ProcessBuilder(command, param, execString);
			builder.redirectErrorStream(true);
			try
			{
				Process process = builder.start();
				InputStream @is = process.InputStream;
				BufferedReader reader = new BufferedReader(new InputStreamReader(@is));

				string line = null;
				StringBuilder output = new StringBuilder();
				while ((line = reader.readLine()) != null)
				{
					output.Append(line).Append(System.getProperty("line.separator"));
				}
				return output.ToString();
			}
			catch (Exception)
			{
				return "";
			}
		}

		public static void sysFileWrite(string path)
		{
			try
			{
				IList<string> lines = Arrays.asList("Test Line 1", "Test Line 2");
				Path file = Paths.get("the-file-name.txt");
				Files.write(file, lines, Charset.forName("UTF-8"));
			}
			catch (IOException e)
			{
				JuliarLogger.log(e.Message + " sysFileWrite error");
			}
		}

		public static void sysPrintLine(string @string)
		{
			Console.WriteLine(stripQuotes(@string));

		}

		public static void sysPrint(string @string)
		{
			Console.Write(stripQuotes(@string));
		}

		public static string stripQuotes(string s)
		{
			if (s.StartsWith("\"", StringComparison.Ordinal) && s.EndsWith("\"", StringComparison.Ordinal) || s.StartsWith("'", StringComparison.Ordinal) && s.EndsWith("'", StringComparison.Ordinal) || s.StartsWith("`", StringComparison.Ordinal) && s.EndsWith("`", StringComparison.Ordinal))
			{
				if (s.Length == 1)
				{
					return s;
				}
				return (s.Substring(1, s.Length - 1 - 1));
			}

			return s;
		}

		public static long sysAvailableMemory()
		{
			Runtime rt = Runtime.Runtime;
			long total = rt.totalMemory();
			long free = rt.freeMemory();
			long freeMemory = total - free;

			return freeMemory;
		}

		public static char[] sysGetByteFromString(string s)
		{
			return s.ToCharArray();
		}
	}

}