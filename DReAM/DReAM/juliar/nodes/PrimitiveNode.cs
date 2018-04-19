using System.Diagnostics;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by Don on 12/24/2016.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class PrimitiveNode extends NodeImpl
	public class PrimitiveNode : NodeImpl
	{
		private string primitiveName;
		private IntegralTypeNode primitiveArgument;

		public virtual string PrimitiveName
		{
			get
			{
				return primitiveName;
			}
		}

		public virtual IntegralTypeNode GetPrimitiveArgument
		{
			get
			{
				return primitiveArgument;
			}
		}

		public virtual IntegralType ArgumentType
		{
			get
			{
				return primitiveArgument.IntegralType;
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.PrimitiveType;
			}
		}

		public override IntegralType IntegralType
		{
			get
			{
				if (this.Instructions.Count > 0)
				{
					FinalNode finalNode = (FinalNode) this.Instructions[0];
					return finalNode.IntegralType;
				}
    
				Debug.Assert(true);
				return null;
			}
		}
	}

}