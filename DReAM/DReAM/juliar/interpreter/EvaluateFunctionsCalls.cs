using System;
using System.Collections.Generic;

namespace com.juliar.interpreter
{

	using com.juliar.nodes;


	public class EvaluateFunctionsCalls
	{

		public static IList<Node> evalUserDefinedFunctionCall(Node n, Interpreter callback)
		{
			UserDefinedTypeFunctionReferenceNode userDefinedTypeFunctionReferenceNode = (UserDefinedTypeFunctionReferenceNode)n;
			//return evalFunctionCall( userDefinedTypeFunctionReferenceNode.getFuncCallNode() );
			return new List<>();
		}

		public static IList<Node> evalFunctionCall(Node node, ActivationFrameStack activationFrame, string mainFunctionName, IDictionary<string, Node> functionNodeMap, Interpreter callback)
		{
			FunctionCallNode functionCallNode = (FunctionCallNode)node;
			string functionToCall = functionCallNode.functionName();

			//ActivationFrame evalFrame = activationFrameStack.pop();
			bool isPrimitive = EvaluatePrimitives.evalIfPrimitive(node, activationFrame.peek(), callback);
			//activationFrameStack.push( evalFrame );
			if (isPrimitive)
			{
				return new List<>();
			}

			// main should only be called from the compliationUnit
			if (functionCallNode.Equals(mainFunctionName))
			{
				return new List<>();
			}

			FunctionDeclNode functionDeclNode = (FunctionDeclNode)functionNodeMap[functionToCall];
			if (functionDeclNode != null)
			{
				ActivationFrame frame = new ActivationFrame();
				frame.frameName = functionToCall;

				IList<VariableNode> sourceVariables = new List<VariableNode>();
				IList<VariableDeclarationNode> targetVariables = new List<VariableDeclarationNode>();

				foreach (Node v in node.Instructions)
				{
					if (v is VariableNode)
					{
						sourceVariables.Add((VariableNode)v);
					}
				}

				foreach (Node v in functionDeclNode.Instructions)
				{
					if (v is VariableDeclarationNode)
					{
						targetVariables.Add((VariableDeclarationNode)v);
					}
				}

				if (sourceVariables.Count != targetVariables.Count)
				{
					throw new Exception("Source and target variable count do not match");
				}

				// since the function that is getting called can reference the variable using the
				// formal parameters of the function this code will match the calling functions data
				// with the target calling functions variable name.
				for (int i = 0; i < sourceVariables.Count; i++)
				{
					VariableNode variableNode = (VariableNode)targetVariables[0].Instructions[1];
					if (variableNode.integralTypeNode == sourceVariables[i].integralTypeNode)
					{
						frame.variableSet[variableNode.variableName] = activationFrame.peek().variableSet[sourceVariables[i].variableName];
					}
					else
					{
						throw new Exception("data types are not the same");
					}
				}

				activationFrame.push(frame);


				IList<Node> statements = getFunctionStatements(functionDeclNode.Instructions);
				callback.execute(statements);
				activationFrame.pop();

				//activationFrame.push(frame);
				//execute(functionDeclNode.getInstructions());

				return new List<Node>();

			}
			else
			{
				FinalNode primitiveArg = new FinalNode();
				primitiveArg.DataString = functionToCall;
				PrimitiveNode primitiveNode = new PrimitiveNode();
				primitiveNode.addInst(primitiveArg);

				foreach (Node primArgs in node.Instructions)
				{
					if (primArgs is VariableNode || primArgs is IntegralTypeNode)
					{
						primitiveNode.addInst(primArgs);
					}
				}

				return EvaluatePrimitives.evalPrimitives(primitiveNode, activationFrame.peek(), callback);
			}
		}

		private static IList<Node> getFunctionStatements(IList<Node> nodes)
		{
			IList<Node> statements = new List<Node>();
			foreach (Node n in nodes)
			{
				if (n is StatementNode)
				{
					statements.Add(n);
				}
			}

			return statements;
		}

	}

}