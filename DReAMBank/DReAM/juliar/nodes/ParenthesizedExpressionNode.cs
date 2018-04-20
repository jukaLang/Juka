namespace DrEAM.nodes
{

	using ActivationFrame = com.juliar.interpreter.ActivationFrame;
	using Interpreter = com.juliar.interpreter.Interpreter;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static com.juliar.nodes.NodeType.ParenthesizedNode;

	public class ParenthesizedExpressionNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return ParenthesizedNode;
			}
		}

		public override IntegralType IntegralType
		{
			get
			{
				return base.IntegralType;
			}
		}


		public override void EvaluateNode(ActivationFrame frame, Interpreter interpreter)
		{
			foreach (Node n in this.Instructions)
			{
				if (n is BooleanOperatorNode || n is ParenthesizedExpressionNode)
				{
					n.EvaluateNode(frame, interpreter);
				}
				else
				{
					if (n is FinalNode && (!((FinalNode) n).dataString().Equals("(") && !((FinalNode) n).dataString().Equals(")")))
					{
						interpreter.pushOperatorStack(n);
					}
					else if (n is VariableNode || n is FunctionCallNode || n is LiteralNode)
					{
						interpreter.pushOperandStack(n);
					}
				}
			}
		}
	}

}