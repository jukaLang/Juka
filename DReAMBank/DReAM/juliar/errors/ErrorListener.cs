using System.Collections.Generic;

namespace DrEAM.errors
{

	using org.antlr.v4.runtime;


	/// <summary>
	/// Created by Don on 10/28/2016.
	/// </summary>
	public class ErrorListener : BaseErrorListener
	{

		private IList<string> errorList_Renamed = new List<string>();
		public override void syntaxError<T1>(Recognizer<T1> recognizer, object offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			string errorMessage = "(" + line + "," + charPositionInLine + ") error on line " + line + " at column " + charPositionInLine + " " + msg;
			errorList_Renamed.Add(errorMessage);
			((Parser)recognizer).RuleInvocationStack;
			base.syntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
		}

		public virtual IList<string> errorList()
		{
			return errorList_Renamed;
		}
	}

}