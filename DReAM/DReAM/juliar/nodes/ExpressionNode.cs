using System;
using System.Collections.Generic;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 3/28/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class ExpressionNode extends NodeImpl
	public class ExpressionNode : NodeImpl
	{
		public ExpressionNode()
		{
			Console.WriteLine("ExpressionNode");
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.ExpressionType;
			}
		}

		public virtual string VariableName
		{
			get
			{
				if (instructions.Count >= 2 && instructions[0] is VariableDeclarationNode)
				{
					VariableDeclarationNode variableDeclarationNode = (VariableDeclarationNode)instructions[0];
					return variableDeclarationNode.VariableNode.variableName;
				}
    
				throw new Exception("variable does not have a name");
			}
		}

		public override void addInst(Node parent, Node instruction)
		{
			base.addInst(parent, instruction);
		}

		public override void addInst(Node instruction)
		{
			if (instruction is FinalNode)
			{
				((NodeImpl)instruction).SetParentNode(this);
				return;
			}
			base.addInst(instruction);
		}

		public override void addInst(Stack<Node> contextStack, Node instruction)
		{
			base.addInst(contextStack, instruction);
		}
	}

}