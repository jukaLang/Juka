namespace DrEAM.nodes
{

	/// <summary>
	/// Created by dreamey on 6/27/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class UserDefinedTypeVariableReference extends NodeImpl
	public class UserDefinedTypeVariableReference : NodeImpl
	{
		public virtual string ObjectName
		{
			get
			{
				FinalNode finalNode = (FinalNode)instructions[0].Instructions[0];
				return finalNode.dataString();
			}
		}

		public virtual string ObjectTypeName
		{
			get
			{
				//instructions.get
				return "";
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.UserDefinedVariableRefrenceType;
			}
		}
	}

}