namespace DrEAM.nodes
{

	using ActivationFrame = com.juliar.interpreter.ActivationFrame;
	using Interpreter = com.juliar.interpreter.Interpreter;


	/// <summary>
	/// Created by don on 4/1/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class BooleanOperatorNode extends NodeImpl
	public class BooleanOperatorNode : NodeImpl
	{

		private new Interpreter interpreter;

		public override NodeType Type
		{
			get
			{
				return NodeType.BooleanOperatorType;
			}
		}


		public override void EvaluateNode(ActivationFrame frame, Interpreter interpreter)
		{
			bool shouldEvaluate = false;
			foreach (Node n in this.Instructions)
			{
				shouldEvaluate = true;
				if (n is BooleanOperatorNode || n is ParenthesizedExpressionNode)
				{
					n.EvaluateNode(frame, interpreter);
				}
				else
				{
					if (n is FinalNode)
					{
						interpreter.pushOperatorStack(n);
					}
					if (n is VariableNode || n is FunctionCallNode || n is LiteralNode)
					{
						string s = ((VariableNode)n).VariableName;
						interpreter.pushOperandStack(n);
					}
				}
			}

			if (shouldEvaluate)
			{
				interpreter.evaluateExpressionStack();
			}
		}
	}

}