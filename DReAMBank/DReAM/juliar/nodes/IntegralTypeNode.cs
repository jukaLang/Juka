namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 10/28/16.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class IntegralTypeNode extends NodeImpl
	public class IntegralTypeNode : NodeImpl
	{
		private const long serialVersionUID = 321323218;
		private FinalNode objectData;
		private string integralName;

		public IntegralTypeNode(FinalNode terminalNode, string name)
		{
			objectData = terminalNode;
			integralName = name;
		}

		public IntegralTypeNode()
		{

		}

		public override IntegralType IntegralType
		{
			get
			{
				return objectData.IntegralType;
			}
		}

		public virtual string IntegralName
		{
			get
			{
				return integralName;
			}
		}

		public virtual FinalNode IntegralValue
		{
			get
			{
				return null;
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.IntegralTypeType;
			}
		}

	}

}