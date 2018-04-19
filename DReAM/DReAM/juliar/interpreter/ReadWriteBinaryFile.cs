namespace com.juliar.interpreter
{

	using InstructionInvocation = DrEAM.codegenerator.InstructionInvocation;
	using JuliarLogger = DrEAM.errors.JuliarLogger;

	/// <summary>
	/// Created by donre on 4/28/2017.
	/// </summary>
	public class ReadWriteBinaryFile
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String inputFile, com.juliar.codegenerator.InstructionInvocation invocation) throws IOException
		public virtual void write(string inputFile, InstructionInvocation invocation)
		{
			string fileName = getLibiraryName(inputFile);

			try (OutputStream ostream = new FileOutputStream(fileName); ObjectOutputStream p = new ObjectOutputStream(ostream)
		   )
		   {
				p.writeObject(invocation);
				p.flush();
		   }
			catch (IOException e)
			{
				JuliarLogger.log(e);
			}
		}

		public virtual InstructionInvocation read(string inputFile)
		{
			string libName = getLibiraryName(inputFile);
			InstructionInvocation invocation = null;
			object @object = null;

			try (FileInputStream istream = new FileInputStream(libName); ObjectInputStream s = new ObjectInputStream(istream)
		   )
		   {
				@object = s.readObject();
				if (@object is InstructionInvocation)
				{
					invocation = (InstructionInvocation) @object;
				}
		   }
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IOException | ClassNotFoundException e)
			{
				JuliarLogger.log(e);
			}
			return invocation;
		}


		private string getLibiraryName(string inputFile)
		{
			string fileName = inputFile;
			char[] pathSeperator = new char[]{System.IO.Path.PathSeparator};

			if (fileName.Contains(new string(pathSeperator)))
			{
				int lastIndexOf = fileName.LastIndexOf(pathSeperator[0]);
				fileName = fileName.Substring(lastIndexOf);
			}

			int extensionIndex = fileName.LastIndexOf('.');
			fileName = fileName.Substring(0, extensionIndex);

			if (fileName != null)
			{
				fileName += ".lib";
			}
			return fileName;
		}
	}

}