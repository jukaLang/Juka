namespace DrEAM.nodes
{

	public class UserDefinedTypeFunctionReferenceNode : NodeImpl
	{

		public virtual string ClassName
		{
			get
			{
				UserDefinedTypeNameNode userDefinedTypeNameNode = (UserDefinedTypeNameNode) instructions[0];
				return userDefinedTypeNameNode.TypeName;
			}
		}

		public virtual FunctionCallNode FuncCallNode
		{
			get
			{
				return (FunctionCallNode)instructions[2];
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.UserDefinedFunctionReferenceType;
			}
		}
	}

}