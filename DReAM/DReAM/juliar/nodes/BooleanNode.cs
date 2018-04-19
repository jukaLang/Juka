using System;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by don on 4/1/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class BooleanNode extends NodeImpl
	public class BooleanNode : NodeImpl
	{
		private const long serialVersionUID = 321323215;
		private Node booleanOperatorNode = null;

		private FinalNode finalNodeRvalue = null;

		public virtual void determineBooleanComparisionType()
		{
			if (instructions.Count == 3)
			{
				booleanOperatorNode = instructions[1];


				Node rvalue = instructions[2];
				if (instructions[2] is PrimitiveNode)
				{
					finalNodeRvalue = (FinalNode) instructions[2].Instructions[0];
				}

				/*if ( rvalue instanceof FinalNode ){
				    finalNodeRvalue = finalNodeRvalue;
				}*/

			}
		}

		public virtual NodeType Lvalue
		{
			get
			{
				return NodeType.VariableType;
			}
		}

		public virtual NodeType ComparisionType
		{
			get
			{
			   if (booleanOperatorNode != null && booleanOperatorNode is EqualEqualSignNode)
			   {
					return NodeType.EqualEqualType;
			   }
    
				throw new Exception("invaild comparision type");
			}
		}

		public virtual NodeType Rvalue
		{
			get
			{
				return NodeType.FinalType;
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.BooleanType;
			}
		}
	}

}