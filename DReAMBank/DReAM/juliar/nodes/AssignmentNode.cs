namespace DrEAM.nodes
{


	/// <summary>
	/// Created by donreamey on 1/7/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class AssignmentNode extends NodeImpl implements IContextInfo
	public class AssignmentNode : NodeImpl, IContextInfo
	{
		private VariableNode variableNode;

		public AssignmentNode(VariableNode v)
		{
			variableNode = v;
		}

		public virtual VariableNode VariableNode
		{
			get
			{
				return variableNode;
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.AssignmentType;
			}
		}
	}

}