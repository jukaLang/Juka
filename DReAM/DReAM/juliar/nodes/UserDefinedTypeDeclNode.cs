namespace DrEAM.nodes
{

	/// <summary>
	/// Created by don on 6/10/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class UserDefinedTypeDeclNode extends NodeImpl
	public class UserDefinedTypeDeclNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return NodeType.UserDefinedDeclarationType;
			}
		}
	}

}