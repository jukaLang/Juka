using System.Collections.Generic;

namespace DrEAM.nodes
{

	using ActivationFrame = DrEAM.interpreter.ActivationFrame;
	using Interpreter = DrEAM.interpreter.Interpreter;


	/// <summary>
	/// Created by donreamey on 10/21/16.
	/// </summary>
	public interface Node
	{
		void addInst(Node parent, Node instruction);

		void addInst(Node instruction);

		void addInst(Stack<Node> contextStack, Node instruction);

		IList<Node> Instructions {get;}

		NodeType Type {get;}

		IntegralType IntegralType {get;}

		IntegralType VariableTypeByIntegralType {set;}

		string VariableType {set;}

		//void EvaluateNode(Node node);

		void EvaluateNode(ActivationFrame frame, Interpreter interpreter);

	}

}