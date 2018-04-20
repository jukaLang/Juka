namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 10/21/16.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class BinaryNode extends NodeImpl
	public class BinaryNode : NodeImpl
	{
		private IntegralTypeNode integralTypeNode;



		public BinaryNode()
		{
		}


		public virtual IntegralTypeNode data()
		{
			return integralTypeNode;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.BinaryType;
			}
		}

	}


}