using System;
using System.Collections.Generic;

namespace DrEAM.juliarrepl
{

	using com.nire4j.repl;


	/// <summary>
	/// Created by donreamey on 11/10/16.
	/// </summary>
	public class JRepl : replTerminal
	{
		public static void Main(string[] args)
		{
			if (args[0] != null && (args[0].Equals("true", StringComparison.CurrentCultureIgnoreCase) || args[0].Equals("false", StringComparison.CurrentCultureIgnoreCase)))
			{
				new JRepl(Convert.ToBoolean(args[0]));
			}
			else
			{
				new JRepl(true);
			}
		}
		public JRepl() : base()
		{
		}

		public JRepl(bool isDebug) : base(isDebug)
		{
		}

		public override void printStartUpMessage()
		{
			//TODO Nothing?
		}

		public override IList<string> repl(ByteArrayInputStream byteArrayInputStream)
		{
			return Collections.emptyList();
		}

		public override void printEndMessage()
		{
			//TODO Nothing?
		}
	}

}