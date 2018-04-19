namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 10/21/16.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class LineNode extends NodeImpl
	public class LineNode : NodeImpl
	{
		public string lastersik;
		public BinaryNode binaryNode;
		public string rastersisk;

		public override NodeType Type
		{
			get
			{
				return NodeType.LineNodeType;
			}
		}
	}

}