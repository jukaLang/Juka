using System;
using System.Collections.Generic;

namespace com.juliar.interpreter
{

	using com.juliar.nodes;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static com.juliar.nodes.IntegralType.*;

	/// <summary>
	/// Created by donreamey on 3/28/17.
	/// </summary>
	public class EvaluateAssignments<T>
	{
		private EvaluateAssignments()
		{
		}

		private static Interpreter interpreterCallback = null;

		public static void create(Interpreter interpreter)
		{
			if (interpreterCallback == null)
			{
				interpreterCallback = interpreter;
			}
		}

		public static IList<Node> evalReassignment(Node n, ActivationFrame activationFrame, Interpreter callback)
		{
			if (n != null)
			{
				VariableReassignmentNode node = (VariableReassignmentNode)n;

				VariableNode lValueVariableNode = (VariableNode)node.Instructions[0];
				string variableName = lValueVariableNode.variableName;

				Node rvalueVariableNode = node.Instructions[2];

				NodeType nodeType = rvalueVariableNode.Type;

				activationFrame.variableSet.Remove(variableName);
				switch (nodeType)
				{
					case com.juliar.nodes.NodeType.LiteralType:
						LiteralNode literalNode = (LiteralNode)rvalueVariableNode;
						activationFrame.variableSet[variableName] = literalNode;
						break;
					case com.juliar.nodes.NodeType.VariableType:
						VariableNode variableNode = (VariableNode) rvalueVariableNode;
						activationFrame.variableSet[variableName] = variableNode;
						break;
					case com.juliar.nodes.NodeType.EvaluatableType:
					   if (true)
					   {
							Console.WriteLine("need to add code to evaluate");
							//ArrayList<Node> evaluatableNode = new ArrayList<Node>()
							//evaluatableNode.add( rvalueVariableNode );
							callback.execute(new List<Node>(rvalueVariableNode));
					   }
						break;
				}
			}

			return new List<>();
		}


		public static void functionCallNode(FunctionCallNode functionCallNode, ActivationFrame activationFrame, VariableDeclarationNode variableToAssignTo)
		{
			IList<Node> functionList = new List<Node>();
			functionList.Add(functionCallNode);
			interpreterCallback.execute(functionList);
			assignReturnValueToVariable(activationFrame, variableToAssignTo);
		}

		public static void primitiveInstance(PrimitiveNode primitiveNode, ActivationFrame activationFrame, VariableDeclarationNode variableToAssignTo)
		{
			if (canPrimitiveValueBeAssignedToVar(variableToAssignTo, primitiveNode))
			{
				string variableName;

				if (variableToAssignTo.IntegralType == juserDefined)
				{

					variableName = variableToAssignTo.UserDefinedNode.FullyQualifiedVariableName;
				}
				else
				{
					FinalNode variableNameTerminalNode = (FinalNode) variableToAssignTo.Instructions[1].Instructions[0];
					variableName = variableNameTerminalNode.dataString();
				}

				if (activationFrame.variableSet.ContainsKey(variableName))
				{
					activationFrame.variableSet.Remove(variableName);
				}

				activationFrame.variableSet[variableName] = primitiveNode;
			}
		}

		public static void booleanInstance(BooleanNode booleanNode, ActivationFrame activationFrame, VariableDeclarationNode variableToAssignTo)
		{
			IList<Node> slotList = new List<Node>();
			slotList.Add(booleanNode);
			interpreterCallback.execute(slotList);
			assignReturnValueToVariable(activationFrame, variableToAssignTo);
		}

		public static void commandInstance(CommandNode commandNode, ActivationFrame activationFrame, VariableDeclarationNode variableToAssignTo)
		{
			IList<Node> slotList = new List<Node>();
			slotList.Add(commandNode);
			interpreterCallback.execute(slotList);

			FinalNode variableNameTerminalNode = (FinalNode) variableToAssignTo.Instructions[1].Instructions[0];

			string variableName = variableNameTerminalNode.dataString();

			if (activationFrame.variableSet.ContainsKey(variableName))
			{
				activationFrame.variableSet.Remove(variableName);
			}

			activationFrame.variableSet[variableName] = activationFrame.peekReturnNode();
			//activationFrame.returnNode = null;
		}

		public static IList<Node> evalVariableDeclWithAssignment(Node n, ActivationFrameStack activationFrameStack, string mainName, IDictionary<string, Node> functionNodeMap)
		{
			VariableDeclarationNode variableDeclarationNode = (VariableDeclarationNode)n;
			IList<Node> instructionsToReturnAndExecute = new List<Node>();
			IList<Node> instructions = variableDeclarationNode.Instructions;
			KeywordNode keywordNode = variableDeclarationNode.KeyWordNode;
			Node rightHandSide = null;
			if (variableDeclarationNode.DeclarationWithAssignment)
			{
				if (!variableDeclarationNode.OperatorEqualSign)
				{
					throw new Exception("Invalid operator for expression");
				}

				rightHandSide = variableDeclarationNode.RightValue;

				switch (rightHandSide.Type)
				{
					case LiteralType:
						if (rightHandSide is LiteralNode)
						{
							LiteralNode literalNode = (LiteralNode) rightHandSide;
							EvaluateAssignments<LiteralNode> literalNodeEvaluateAssignments = new EvaluateAssignments<LiteralNode>();
							if (literalNodeEvaluateAssignments.canLiteralBeAssigned(keywordNode, literalNode))
							{
								if (instructions[1] is VariableNode)
								{
									VariableNode variableNode = (VariableNode) instructions[1];
									if (activationFrameStack.peek().variableSet.ContainsKey(variableNode.variableName))
									{
										throw new Exception("Variable already declared");
									}
									else
									{
										activationFrameStack.peek().variableSet[variableNode.variableName] = rightHandSide;
									}
								}
							}

						}
						break;
					case FunctionaCallType:
						instructionsToReturnAndExecute = EvaluateFunctionsCalls.evalFunctionCall(rightHandSide, activationFrameStack, mainName, functionNodeMap, interpreterCallback);
						if (activationFrameStack.peek().parameterStack.Count > 0)
						{
							VariableNode variableNode = (VariableNode) instructions[1];
							if (activationFrameStack.peek().variableSet.ContainsKey(variableNode.variableName))
							{
								throw new Exception("Variable already declared");
							}
							else
							{
								activationFrameStack.peek().variableSet[variableNode.variableName] = activationFrameStack.peek().parameterStack.Pop();
							}
						}

					break;
				}
			}

			return instructionsToReturnAndExecute;

		}

		public static IList<Node> evalAssignment(Node n, ActivationFrame activationFrame, Interpreter calback)
		{
			AssignmentNode assignmentNode = (AssignmentNode)n;
			IList<Node> instructions = assignmentNode.Instructions;

			const int varDeclIndex = 0;
			const int equalSignIndex = 1;
			const int primtiveIndex = 2;
			VariableDeclarationNode variableToAssignTo = (VariableDeclarationNode)instructions[varDeclIndex];

			// | zero             | one       | two
			// | variableDecl     | EqualSign | Primitive
			// | int variableName | =         | 3

			if (instructions[equalSignIndex] is EqualSignNode)
			{
				object rvalue = instructions[primtiveIndex];

				if (rvalue is FunctionCallNode)
				{
					functionCallNode((FunctionCallNode) rvalue, activationFrame, variableToAssignTo);
				}
				if (rvalue is PrimitiveNode)
				{
					primitiveInstance((PrimitiveNode) rvalue, activationFrame, variableToAssignTo);
				}
				if (rvalue is BooleanNode)
				{
					booleanInstance((BooleanNode) rvalue, activationFrame, variableToAssignTo);
				}
				if (rvalue is CommandNode)
				{
					commandInstance((CommandNode) rvalue, activationFrame, variableToAssignTo);
				}
			}
			return new List<>();
		}

		private static void assignReturnValueToVariable(ActivationFrame activationFrame, VariableDeclarationNode variableToAssignTo)
		{
			if (activationFrame.peekReturnNode() != null)
			{
				VariableNode variableNode = (VariableNode)variableToAssignTo.Instructions[1];
				if (activationFrame.variableSet.ContainsKey(variableNode.variableName))
				{
					activationFrame.variableSet.Remove(variableNode.variableName);
				}

				activationFrame.variableSet[variableNode.variableName] = activationFrame.popNode();
			}
		}

		public virtual bool canLiteralBeAssigned<T>(KeywordNode keyword, T t) where T : NodeImpl
		{
			if (keyword.Instructions[0] is FinalNode)
			{
				FinalNode finalNode = (FinalNode)keyword.Instructions[0];
				string dataType = finalNode.dataString();

				string rvalueDataString = ((FinalNode)t.Instructions.get(0)).dataString();

				switch (finalNode.IntegralType)
				{
					case jinteger:
					{
						try
						{
							Convert.ToInt32(rvalueDataString);
						}
						catch (NumberFormatException)
						{
							throw new Exception("invalid r-Value");
						}

						return true;
					}
				}
			}

			return false;
		}


		public static bool canPrimitiveValueBeAssignedToVar(VariableDeclarationNode lvalue, PrimitiveNode rvalue)
		{
			FinalNode rvalueTerminal = (FinalNode) rvalue.Instructions[0];

			VariableNode variableNode;

			if (juserDefined == lvalue.IntegralType)
			{
				variableNode = lvalue.UserDefinedNode.VariableNode;
			}
			else
			{
				variableNode = (VariableNode) lvalue.Instructions[1];
			}
			string data = rvalueTerminal.dataString();

			try
			{
				switch (variableNode.IntegralType)
				{
					case jinteger:
						return true;
					case jdouble:
						return true;
					case jfloat:
						return true;
					case jlong:
						return true;
					case jstring:
						return true;
					case jobject:
						return false;
					case jboolean:
						return Convert.ToBoolean(data);
					case juserDefined:
						return true;
					default:
						return false;
				}
			}
			catch (NumberFormatException)
			{
				return false;
			}
		}
	}

}