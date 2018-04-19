using System;
using System.Collections.Generic;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by don on 3/25/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class VariableReassignmentNode extends NodeImpl
	public class VariableReassignmentNode : NodeImpl
	{
		public VariableReassignmentNode()
		{
			Console.WriteLine("variable reassignmnent");
		}

		public virtual Node RvalueType
		{
			get
			{
				return instructions[2].Instructions[0];
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.VariableReassignmentType;
			}
		}

		public override void addInst(Node parent, Node instruction)
		{
			base.addInst(parent, instruction);
		}

		/*
		@Override
		public void addInst(Node instruction) {
		    super.addInst(instruction);
		}
		*/

		public override void addInst(Stack<Node> contextStack, Node instruction)
		{
			base.addInst(contextStack, instruction);
		}
	}

}