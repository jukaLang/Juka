using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 1/18/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class VariableDeclarationNode extends NodeImpl
	public class VariableDeclarationNode : NodeImpl
	{
		private static int ASSIGNMENT_SIZE = 4;
		private static int OPERATOR_SLOT = 3;
		public string type;

		public VariableDeclarationNode()
		{
			Console.WriteLine("variableDeclarationNode");
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.VariableDeclarationType;
			}
		}

		public override IntegralType IntegralType
		{
			get
			{
				if (this.Instructions.Count > 0)
				{
					if (this.Instructions[0] is KeywordNode)
					{
						KeywordNode keywordNode = (KeywordNode) this.Instructions[0];
						if (keywordNode.Instructions.Count > 0)
						{
							return keywordNode.IntegralType;
						}
					}
					if (this.Instructions[0] is UserDefinedTypeNode)
					{
						UserDefinedTypeNode keywordNode = (UserDefinedTypeNode) this.Instructions[0];
						return keywordNode.Instructions[2].IntegralType;
					}
				}
    
				Debug.Assert(true);
				return null;
			}
		}

		public virtual UserDefinedTypeNode UserDefinedNode
		{
			get
			{
				Debug.Assert(this.Instructions.Count > 0);
				return (UserDefinedTypeNode)this.Instructions[0];
			}
		}

		public virtual bool DeclarationWithAssignment
		{
			get
			{
				return this.Instructions.Count >= ASSIGNMENT_SIZE;
			}
		}

		public virtual KeywordNode KeyWordNode
		{
			get
			{
				return (KeywordNode) this.Instructions[0];
			}
		}

		public virtual bool OperatorEqualSign
		{
			get
			{
				if (this.Instructions.Count >= OPERATOR_SLOT)
				{
					return this.Instructions[OPERATOR_SLOT - 1] is EqualSignNode;
				}
    
				return false;
			}
		}

		public virtual Node RightValue
		{
			get
			{
				if (this.Instructions.Count == ASSIGNMENT_SIZE)
				{
					return this.Instructions[OPERATOR_SLOT];
				}
    
				return new NodeImplAnonymousInnerClassHelper(this);
			}
		}

		private class NodeImplAnonymousInnerClassHelper : NodeImpl
		{
			private readonly VariableDeclarationNode outerInstance;

			public NodeImplAnonymousInnerClassHelper(VariableDeclarationNode outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override NodeType Type
			{
				get
				{
					return NodeType.Nul;
				}
			}
		}

		public virtual VariableNode VariableNode
		{
			get
			{
				return (VariableNode) instructions[1];
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