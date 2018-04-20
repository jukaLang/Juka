namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 2/10/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class ReturnValueNode extends NodeImpl
	public class ReturnValueNode : NodeImpl
	{
		private string typeName_Renamed;

		public virtual string typeName()
		{
			return typeName_Renamed;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.ReturnValueType;
			}
		}

	}

}