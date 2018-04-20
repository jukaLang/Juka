using System.Collections.Generic;

namespace DrEAM.nodes
{


	/// <summary>
	/// Created by donreamey on 10/21/16.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class StatementNode extends NodeImpl
	public class StatementNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return NodeType.StatementType;
			}
		}


		public virtual IList<string> findAllVariablesInStatement()
		{
			int size = instructions.Count;
			IList<string> variables = new List<string>(size);

			for (int i = 0; i < size; i++)
			{
				if (instructions[i] is ExpressionNode)
				{
					ExpressionNode expressionNode = (ExpressionNode)instructions[i];

					variables.Add(expressionNode.VariableName);
				}
			}

			return variables;
		}

	}

}