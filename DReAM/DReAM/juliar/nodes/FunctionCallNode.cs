using System.Diagnostics;
using System.Collections.Generic;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by Don on 1/12/2017.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class FunctionCallNode extends NodeImpl
	public class FunctionCallNode : NodeImpl
	{
		public virtual string functionName()
		{
			IList<Node> nodes = Instructions;
			if (nodes.Count > 0)
			{
				FinalNode functionName = (FinalNode)nodes[0];
				return functionName.dataString();

			}
			Debug.Assert(true);
			return null;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.FunctionaCallType;
			}
		}

	}

}