namespace DrEAM.nodes
{

	using ActivationFrame = com.juliar.interpreter.ActivationFrame;

	/// <summary>
	/// Created by Don on 1/15/2017.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class VariableNode extends NodeImpl
	public class VariableNode : NodeImpl
	{
		private const long serialVersionUID = 321323217;
		private Node parent;
		public string variableName;
		public IntegralTypeNode integralTypeNode;


		public virtual Node Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}



		public VariableNode(string vName)
		{
			variableName = vName;
		}

		public virtual string VariableName
		{
			get
			{
				FinalNode finalNode = (FinalNode) Instructions[0];
				sbyte[] bytes = finalNode.FinalNodeBytes;
    
				return StringHelperClass.NewString(bytes);
			}
		}

		public virtual IntegralTypeNode IntegralTypeNode
		{
			set
			{
				integralTypeNode = value;
			}
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.VariableType;
			}
		}


		public virtual FinalNode FinalNode
		{
			get
			{
				return (FinalNode) this.Instructions[0];
			}
		}

		public override object getRealValue(ActivationFrame frame)
		{
			string variableName = ((FinalNode)Instructions[0]).dataString();
			if (frame.variableSet.ContainsKey(variableName))
			{
				return ((FinalNode)frame.variableSet[variableName].Instructions[0]).dataString();
			}

			return "";
		}
	}

}