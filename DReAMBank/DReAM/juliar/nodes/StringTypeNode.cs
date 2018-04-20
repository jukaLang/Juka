namespace DrEAM.nodes
{

	/// <summary>
	/// Created by dreamey on 6/20/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class StringTypeNode extends NodeImpl
	public class StringTypeNode : NodeImpl
	{

		public virtual string StringData
		{
			get
			{
				return ((FinalNode)instructions[0]).dataString();
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.StringType;
			}
		}
	}

}