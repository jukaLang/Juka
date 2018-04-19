using System;
using System.Collections.Generic;

namespace DrEAM.loaderlinker
{

	using InstructionInvocation = DrEAM.codegenerator.InstructionInvocation;
	using ReadWriteBinaryFile = DrEAM.interpreter.ReadWriteBinaryFile;
	using CompliationUnitNode = DrEAM.nodes.CompliationUnitNode;
	using Node = DrEAM.nodes.Node;


	/// <summary>
	/// Created by dreamey on 5/12/17.
	/// </summary>
	public class LoadLink
	{
		public static InstructionInvocation loadAndLink(string[] filesToLoad)
		{
			int fileCount = filesToLoad.Length;

			ReadWriteBinaryFile read = new ReadWriteBinaryFile();
			InstructionInvocation[] instructionInvocations = new InstructionInvocation[fileCount];
			int count = 0;

			foreach (string file in filesToLoad)
			{
				instructionInvocations[count] = read.read(file);
				if (instructionInvocations[count].InstructionList.Count == 0)
				{
					throw new Exception("Invalid library. Make sure there were no compilation errors");
				}
				count++;
			}

			InstructionInvocation instructionInvocation = link(instructionInvocations);
			//read.write( "a.out",  );
			return instructionInvocation;
		}


		/*
		Finds all function maps and combines into one function map. Looks for multiple main methods.
	
		 */
		private static InstructionInvocation link(InstructionInvocation[] instructionInvocations)
		{
			InstructionInvocation firstInvocation = instructionInvocations[0];

			CompliationUnitNode compliationUnitNode = (CompliationUnitNode) firstInvocation.InstructionList[0];

			Dictionary<string, Node> functionNodeMap = new Dictionary<string, Node>();

//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			functionNodeMap.putAll(firstInvocation.FunctionNodeMap);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			long oneMainFunction = functionNodeMap.Keys.stream().filter(f => "main".Equals(f)).count();

			if (instructionInvocations.Length > 0)
			{
				for (int i = 1; i < instructionInvocations.Length; i++)
				{
					compliationUnitNode.Instructions.AddRange(instructionInvocations[i].InstructionList);

//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					oneMainFunction += instructionInvocations[i].FunctionNodeMap.Keys.stream().filter(f => "main".Equals(f)).count();

//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
					functionNodeMap.putAll(instructionInvocations[i].FunctionNodeMap);
				}
			}

			if (oneMainFunction == 1)
			{
				IList<Node> inst = new List<Node>();
				inst.Add(compliationUnitNode);
				return new InstructionInvocation(inst, functionNodeMap);
			}

			if (oneMainFunction > 1)
			{
				throw new Exception("Multiple main methods were found during linking");
			}


			throw new Exception("No main functions were found during linking");
		}
	}

}