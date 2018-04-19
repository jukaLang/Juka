using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace com.juliar.interpreter
{

	using JuliarLogger = com.juliar.errors.JuliarLogger;
	using com.juliar.nodes;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static com.juliar.pal.Primitives.*;

	/// <summary>
	/// Created by donreamey on 3/28/17.
	/// </summary>
	internal class EvaluatePrimitives
	{
		private static IList<string> primitiveFunctions = new List<string>(Arrays.asList("print", "__getByteFromString", "printLine", "availableMemory", "sysExec", "fileOpen"));

		private EvaluatePrimitives()
		{

		}

		public static bool evalIfPrimitive(Node n, ActivationFrame activationFrame, Interpreter calback)
		{
			string functionName = ((FinalNode) n.Instructions[0]).dataString();
			if (primitiveFunctions.Contains(functionName))
			{
				evalPrimitives(n, activationFrame, calback);
				return true;
			}

			return false;
		}

		public static IList<Node> evalPrimitives(Node n, ActivationFrame activationFrame, Interpreter calback)
		{
			string functionName = ((FinalNode) n.Instructions[0]).dataString();
			FinalNode finalNode = new FinalNode();

			switch (functionName)
			{
				case "print":
					printLine(activationFrame, functionName, n.Instructions[2]);
					break;
				case "__getByteFromString":
					getByteFromString(activationFrame, n.Instructions[1], n.Instructions[2]);
					break;
				case "printLine":
					printLine(activationFrame, functionName, n.Instructions[1]);
					break;
				case "fileOpen":
					string data = fileOpen(n.Instructions[2]);
					finalNode.DataString = data;
					finalNode.VariableTypeByIntegralType = IntegralType.jstring;
					activationFrame.pushReturnNode(finalNode);
					//activationFrame.returnNode = finalNode;
					break;
				case "sysExec":
					string ex = sysExec(n.Instructions[2]);
					finalNode.DataString = ex;
					finalNode.VariableTypeByIntegralType = IntegralType.jstring;
					activationFrame.pushReturnNode(finalNode);
					//activationFrame.returnNode = finalNode;
					break;
				case "availableMemory":
					long value = availableMemory();
					finalNode.DataString = value;
					finalNode.VariableTypeByIntegralType = IntegralType.jlong;
					activationFrame.pushReturnNode(finalNode);
					//activationFrame.returnNode = finalNode;
					break;
				default:
					JuliarLogger.log("function " + functionName + " does not exist");
					break;
			}

			return new List<>();
		}

		private static string fileOpen(Node argumentNode)
		{
			if (argumentNode is FinalNode)
			{
				FinalNode finalNode = (FinalNode) argumentNode;
				return sysFileOpen(finalNode.dataString());
			}

			return "";
		}

		public static string sysExec(Node argumentNode)
		{
			if (argumentNode is FinalNode)
			{
				FinalNode finalNode = (FinalNode) argumentNode;
				return com.juliar.pal.Primitives.sysExec(finalNode.dataString());
			}
			return "";
		}

		private static long availableMemory()
		{
			return sysAvailableMemory();
		}

		private static void getByteFromString(ActivationFrame activationFrame, Node argumentNode, Node index)
		{
			string variableName = ((VariableNode) argumentNode).variableName;
			object variable = activationFrame.variableSet[variableName];

			if (variable is FinalNode)
			{
				char[] array = sysGetByteFromString(((FinalNode) variable).dataString());

				FinalNode finalNode = new FinalNode();

				string argTwoVariableName = ((VariableNode) index).variableName;
				object argTwo = activationFrame.variableSet[argTwoVariableName];

				FinalNode argumentTwo = null;

				if (argTwo is PrimitiveNode)
				{
					argumentTwo = (FinalNode) activationFrame.variableSet[argTwoVariableName].Instructions[0];
				}
				else if (argTwo is FinalNode)
				{
					argumentTwo = (FinalNode) activationFrame.variableSet[argTwoVariableName];
				}

				assert(argumentTwo != null ? argumentTwo.dataString() : null) != null;
				int parsedIndex = Convert.ToInt32(argumentTwo.dataString());

				if (parsedIndex > array.Length)
				{
					throw new Exception("\r\nJuliar runtime exception - Index out of bounds accessing variable - '" + variableName + "'");
				}
				if (parsedIndex < array.Length)
				{
					finalNode.DataString = array[parsedIndex];
					activationFrame.pushReturnNode(finalNode);
					//activationFrame.returnNode = finalNode;
				}
			}
		}

		private static void printLine(ActivationFrame activationFrame, string functionName, Node argumentNode)
		{
			FinalNode finalNode = null;

			if (argumentNode == null)
			{
				if (activationFrame.peekReturnNode() != null)
				{
					//argumentNode = activationFrame.returnNode;
					argumentNode = activationFrame.popNode();
				}
			}

			switch (argumentNode.Type)
			{
				case LiteralType:
					finalNode = (FinalNode) argumentNode.Instructions[0];
					break;
				case VariableType:
					string variableName = ((VariableNode)argumentNode).variableName;
					Node tempVariableNode = activationFrame.variableSet[variableName];
					if (tempVariableNode == null)
					{
						// a variable has been declared and not initazlized
						finalNode = new FinalNode();
						break;
					}
					if (tempVariableNode is VariableNode)
					{
						string name = ((VariableNode)tempVariableNode).variableName;
						if (activationFrame.variableSet.ContainsKey(name))
						{
							finalNode = (FinalNode) activationFrame.variableSet[name].Instructions[0];
						}
					}
					else
					{
						printLine(activationFrame, functionName, tempVariableNode);
						return;
					}
					break;
				case FinalType:
					finalNode = (FinalNode)argumentNode;
					break;
			}

			if (finalNode == null)
			{
				dumpFrameVariables(activationFrame);
				Debug.Assert(finalNode != null, "the final node cannot be null");
			}

			string stringToPrint = finalNode.dataString();

			if (functionName.Equals("printLine"))
			{
				sysPrintLine(stringToPrint);
				return;
			}
			if (functionName.Equals("print"))
			{
				sysPrint(stringToPrint);
				return;
			}
		}

		private static void dumpFrameVariables(ActivationFrame frame)
		{
			Dictionary<string, com.juliar.nodes.Node>.KeyCollection keys = frame.variableSet.Keys;
			foreach (string s in keys)
			{
				Console.WriteLine(string.Format("key {0} = value {1}", s, frame.variableSet[s].ToString()));
			}
		}

	}



}