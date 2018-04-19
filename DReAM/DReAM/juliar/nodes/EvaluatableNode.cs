using System;
using System.Collections.Generic;

namespace DrEAM.nodes
{

	using ActivationFrame = com.juliar.interpreter.ActivationFrame;
	using Interpreter = com.juliar.interpreter.Interpreter;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static com.juliar.nodes.NodeType.EvaluatableType;

	public class EvaluatableNode : NodeImpl
	{
		public override void addInst(Node parent, Node instruction)
		{
			base.addInst(parent, instruction);
		}

		public override void addInst(Node instruction)
		{
			base.addInst(instruction);
		}

		public override void addInst(Stack<Node> contextStack, Node instruction)
		{
			base.addInst(contextStack, instruction);
		}

		public EvaluatableNode()
		{
			Console.WriteLine("evaluatableNode");
		}

		public override NodeType Type
		{
			get
			{
				return EvaluatableType;
    
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
			}

			/*
			if (n instanceof FinalNode) {
			    interpreter.pushOperatorStack(n);
			} else if (n instanceof VariableNode || n instanceof FunctionCallNode || n instanceof LiteralNode) {
			    interpreter.pushOperandStack(n);
			}
			if (n.getInstructions().size() == 3) {
			    EvaluateNode(n.getInstructions().get(0));
			    EvaluateNode(n.getInstructions().get(1));
			    EvaluateNode(n.getInstructions().get(2));
			}*/


		}
	/*
	    public void evaluateExpression(ActivationFrame frame, Interpreter interpreter) {
	        try {
	
	
	            Node node = this;
	            Node lvalue = null;
	            boolean isEqualEqual;
	            Node operatorTypeNode = null;
	            Node rvalue = null;
	            this.interpreter = interpreter;
	
	
	            // This is ugly code. Need to find a better way to
	            // handle these cases.
	            // Multiple ifs will only cause confusion.
	            FinalNode updatedLvalue = null;
	
	            int instructionCount = getInstructions().size();
	
	            if (instructionCount == 3) {
	
	                lvalue = getInstructions().get(0);
	                operatorTypeNode = getInstructions().get(1);
	                rvalue = getInstructions().get(2);
	
	                EvaluateNode(this);
	
	                // frame.pushReturnNode( booleanNode );
	            }
	        } catch (Exception ex) {
	            Logger.log(ex.getMessage());
	        }
	   }
	   */
	}

}