using System.Collections.Generic;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 10/25/16.
	/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class AggregateNode extends NodeImpl
	public class AggregateNode : NodeImpl
	{

		private Operation op;
		private IList<IntegralTypeNode> objectData;

		public AggregateNode(Operation operation)
		{
			op = operation;
		}

		public AggregateNode()
		{
		}

		public AggregateNode(Operation operation, IList<IntegralTypeNode> data)
		{
			op = operation;
			objectData = data;
		}

		public virtual AggregateNode makeNode(Operation operation, IList<IntegralTypeNode> data)
		{
			return new AggregateNode(operation, objectData = data);
		}

		public virtual Operation operation()
		{
			return op;
		}

		public virtual IList<IntegralTypeNode> data()
		{
			return objectData;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.AggregateType;
			}
		}

	}
}