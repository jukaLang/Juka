namespace DrEAM.nodes
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static com.juliar.nodes.NodeType.UserDefinedNameType;

	/// <summary>
	/// Created by dreamey on 6/27/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class UserDefinedTypeNameNode extends NodeImpl
	public class UserDefinedTypeNameNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return UserDefinedNameType;
			}
		}

		public virtual string TypeName
		{
			get
			{
				FinalNode finalNode = (FinalNode)instructions[0];
				return finalNode.dataString();
			}
		}
	}

}