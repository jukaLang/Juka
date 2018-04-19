namespace DrEAM.nodes
{

	using ActivationFrame = com.juliar.interpreter.ActivationFrame;

	public class LiteralNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return NodeType.LiteralType;
			}
		}

		public virtual bool? isEqual(LiteralNode literalNode)
		{
			return ((FinalNode)Instructions[0]).dataString().Equals(((FinalNode)literalNode.Instructions[0]).dataString());
		}

		public override object getRealValue(ActivationFrame frame)
		{
			return ((FinalNode)Instructions[0]).dataString();
		}
	}

}