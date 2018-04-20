namespace DrEAM.nodes
{

	/// <summary>
	/// Created by dreamey on 6/26/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class UserDefinedTypeVariableDeclNode extends NodeImpl
	public class UserDefinedTypeVariableDeclNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return NodeType.UserDefinedVariableDeclarationType;
			}
		}

		public virtual string UserDefinedVariableTypeName
		{
			get
			{
				UserDefinedTypeNameNode name = (UserDefinedTypeNameNode) instructions[0];
				return name.TypeName;
			}
		}
	}

}