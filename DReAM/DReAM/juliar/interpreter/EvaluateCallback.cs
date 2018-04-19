using System.Collections.Generic;

namespace com.juliar.interpreter
{

	using Node = com.juliar.nodes.Node;

	internal interface Evaluate
	{
		IList<Node> evaluate(Node node, ActivationFrame frame, Interpreter callback);
	}
}