using System;

namespace DrEAM
{

	using ErrorListener = com.juliar.errors.ErrorListener;
	using JuliarLogger = com.juliar.errors.JuliarLogger;
	using Interpreter = com.juliar.interpreter.Interpreter;
	using JuliarLexer = com.juliar.parser.JuliarLexer;
	using JuliarParser = com.juliar.parser.JuliarParser;
	using SymbolTable = com.juliar.symboltable.SymbolTable;
	using Visitor = com.juliar.vistor.Visitor;
	using CharStream = org.antlr.v4.runtime.CharStream;
	using CharStreams = org.antlr.v4.runtime.CharStreams;
	using CommonTokenStream = org.antlr.v4.runtime.CommonTokenStream;


	public class DReAMMain
	{
		private static ErrorListener errors;
		public static void Main(string[] args)
		{
			try
			{
				compile(args[0]);
			}
			catch (Exception)
			{
				JuliarLogger.log("Something went wrong");
			}
		}

		public static void compile(string s)
		{
			InputStream b = new ByteArrayInputStream(s.GetBytes(StandardCharsets.UTF_8));
			try
			{
				SymbolTable.clearSymbolTable();
				JuliarParser parser = parse(b);

				errors = new ErrorListener();
				parser.addErrorListener(errors);

				// call parse statement.
				// This will parse a single line to validate the syntax

				if (excuteCompiler(parser))
				{
					errors.errorList();
				}

			}
			catch (Exception ex)
			{
				JuliarLogger.log(ex.Message);
			}
		}

		/*
		Will execute the compiler or the interpreter.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static boolean excuteCompiler(com.juliar.parser.JuliarParser parser) throws java.io.IOException
		private static bool excuteCompiler(JuliarParser parser)
		{
			// Calls the parse CompileUnit method
			// to parse a complete program
			// then calls the code generator.

			JuliarParser.CompileUnitContext context = parser.compileUnit();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			Visitor visitor = new Visitor((imports, linesToSkip) =>
			{
			}, true);
			visitor.visit(context);

			if (errors.errorList().Count > 0 || visitor.ErrorList.Count > 0)
			{
				foreach (string error in errors.errorList())
				{
					JuliarLogger.logerr(error);
				}

				foreach (string error in visitor.ErrorList)
				{
					JuliarLogger.logerr(error);
				}

				return true;
			}
			new Interpreter(visitor.instructions());
			return false;
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static com.juliar.parser.JuliarParser parse(java.io.InputStream b) throws java.io.IOException
		private static JuliarParser parse(InputStream b)
		{
			JuliarParser parser;
			CharStream s = CharStreams.fromStream(b);
			JuliarLexer lexer = new JuliarLexer(s);
			CommonTokenStream tokenStream = new CommonTokenStream(lexer);
			parser = new JuliarParser(tokenStream);
			parser.removeErrorListeners();
			return parser;
		}
	}

}