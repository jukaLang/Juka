using System;
using System.Collections.Generic;

namespace DrEAM.nodes
{


	/// <summary>
	/// Created by Don on 1/12/2017.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class FunctionDeclNode extends NodeImpl implements IContextInfo
	public class FunctionDeclNode : NodeImpl, IContextInfo
	{
		private string functionName;

		public FunctionDeclNode() : base()
		{
			Console.WriteLine("creating functionDeclNode");
		}


		public FunctionDeclNode(string funcName, IList<Node> inst) : this()
		{
			functionName = funcName;
			instructions = inst;
		}
		public virtual string FunctionName
		{
			get
			{
				return functionName;
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.FunctionDeclType;
			}
		}

		public override void addInst(Node parent, Node instruction)
		{
			base.addInst(parent, instruction);
		}

		public override void addInst(Node instruction)
		{
			// Throw away assigned instructions since we already know the
			// function name
			if (instruction is StatementNode)
			{
				base.addInst(instruction);
			}

			((NodeImpl)instruction).SetParentNode(this);

			return;
		}

		public override void addInst(Stack<Node> contextStack, Node instruction)
		{
			base.addInst(contextStack, instruction);
		}
	}

}