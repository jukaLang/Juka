using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DrEAM.interpreter
{

	using Node = DrEAM.nodes.Node;
	using VariableNode = DrEAM.nodes.VariableNode;


	/// <summary>
	/// Created by donreamey on 1/18/17.
	/// </summary>
	public class ActivationFrame
	{
		public string frameName;
		public Dictionary<string, Node> variableSet = new Dictionary<string, Node>();
		public Stack<Node> parameterStack = new Stack<Node>();
		public Stack<Node> operandStack = new Stack<Node>();
		private Node returnNode;

		public ActivationFrame()
		{
		}

		public ActivationFrame(string frameName)
		{
			this.frameName = frameName;
		}

		public virtual void pushReturnNode(Node node)
		{
		  parameterStack.Push(node);
		}

		public virtual Node peekReturnNode()
		{
			if (parameterStack.Count == 0)
			{
				Debug.Assert(true, "return node stack is empty");
			}

			return parameterStack.Peek();
		}

		public virtual Node popNode()
		{
			if (parameterStack.Count == 0)
			{
				Debug.Assert(true, "return node stack is empty");
			}

			return parameterStack.Pop();
		}

		public virtual Node getNodeFromFrameByVariableNode(VariableNode node)
		{
			if (variableSet.ContainsKey(node.variableName))
			{
				return variableSet[node.variableName];
			}

			throw new Exception("variable not found");
		}
	}
}