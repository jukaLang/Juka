using System.Collections.Generic;

namespace com.juliar.interpreter
{

	using IntegralTypeNode = com.juliar.nodes.IntegralTypeNode;
	using Node = com.juliar.nodes.Node;
	using VariableNode = com.juliar.nodes.VariableNode;

	public class ActivationFrameStack
	{
		private Stack<ActivationFrame> activationFrameStack = new Stack<ActivationFrame>();

		public virtual void push(ActivationFrame frame)
		{
			// Logger.log ("push [" + frame.frameName + "]");
			activationFrameStack.Push(frame);
		}

		public virtual ActivationFrame pop()
		{
			// Logger.log ("pop [" + activationFrameStack.peek().frameName +"]");
			return activationFrameStack.Pop();
		}

		public virtual ActivationFrame peek()
		{
			//Logger.log ("peek [" + activationFrameStack.peek().frameName +"]");
			return activationFrameStack.Peek();
		}

		public virtual int size()
		{
			if (activationFrameStack.Count > 0)
			{
				return activationFrameStack.Count;
			}

			return 0;
		}

		public virtual Stack<Node> setupReturnValueOnStackFrame(Node rValue)
		{

			int size = activationFrameStack.Count - 1;

			ActivationFrame currentFrame = activationFrameStack.get(size);
			ActivationFrame caller = activationFrameStack.get(size - 1);

			if (caller != null)
			{
				if (rValue is VariableNode && currentFrame.variableSet.ContainsKey(((VariableNode) rValue).variableName))
				{
					caller.pushReturnNode(currentFrame.variableSet[((VariableNode) rValue).variableName]);
				}
				if (rValue is IntegralTypeNode)
				{
					caller.pushReturnNode(rValue);
				}

				activationFrameStack.Push(caller);
				activationFrameStack.Push(currentFrame);

			}

			/*
	
			if (frame.variableSet.containsKey(node.typeName())) {
			    Node variableNode = frame.variableSet.get(node.typeName());
			    returnValueStack.push( variableNode );
			}
			*/

			return new Stack<>();
		}
	}

}