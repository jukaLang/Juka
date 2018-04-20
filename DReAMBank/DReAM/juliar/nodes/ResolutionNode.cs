namespace DrEAM.nodes
{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class ResolutionNode extends NodeImpl
	public class ResolutionNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return NodeType.ResolutionType;
			}
		}

		public override IntegralType IntegralType
		{
			get
			{
				return null;
			}
		}
	}

}