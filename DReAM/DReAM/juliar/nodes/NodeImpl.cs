using System;
using System.Collections.Generic;

namespace DrEAM.nodes
{

	using ActivationFrame = com.juliar.interpreter.ActivationFrame;
	using Interpreter = com.juliar.interpreter.Interpreter;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static com.juliar.nodes.IntegralType.*;

	/// <summary>
	/// Created by Don on 1/13/2017.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public abstract class NodeImpl implements Node, Serializable
	[Serializable]
	public abstract class NodeImpl : Node
	{
		public abstract NodeType Type {get;}
		[NonSerialized]
		protected internal IList<Node> instructions = new List<Node>();
		public Interpreter interpreter;
		public ActivationFrame frame;

		protected internal Node parentNode;

		private IntegralType integralType;


		public NodeImpl()
		{
			//setNodeName();
		}

		public virtual Node ParentNode()
		{
			return parentNode;
		}

		public virtual void SetParentNode(Node node)
		{
			parentNode = node;
		}

		public virtual void addInst(Node parent, Node instruction)
		{
		/*add instruction given parent*/
		}

		public virtual void addInst(Node instruction)
		{
			NodeImpl nodeImpl = (NodeImpl)instruction;
			if (nodeImpl != null && nodeImpl.parentNode == null)
			{
				nodeImpl.parentNode = this;
			}
			instructions.Add(instruction);
		}

		public virtual void addInst(Stack<Node> contextStack, Node instruction)
		{
			Node n = contextStack.Peek();
			n.addInst(instruction);
		}

		public virtual IList<Node> Instructions
		{
			get
			{
				return instructions;
			}
		}

		public virtual IntegralType IntegralType
		{
			get
			{
				return integralType;
			}
		}

		public virtual IntegralType VariableTypeByIntegralType
		{
			set
			{
				integralType = value;
			}
		}

		public virtual string VariableType
		{
			set
			{
				switch (value)
				{
					case "int":
						integralType = jinteger;
						break;
					case "double":
						integralType = jdouble;
						break;
					case "float":
						integralType = jfloat;
						break;
					case "long":
						integralType = jlong;
						break;
					case "string":
						integralType = jstring;
						break;
					case "object":
						integralType = jobject;
						break;
					case "boolean":
						integralType = jboolean;
						break;
					default:
						integralType = juserDefined;
						break;
				}
			}
		}

		public virtual void EvaluateNode(ActivationFrame frame, Interpreter interpreter)
		{
		}

		public virtual IList<Node> ConditionalExpressions
		{
			get
			{
				IList<Node> inst = Instructions;
				IList<Node> conditionalExpressions = new List<Node>();
    
				int instCount = inst.Count;
				for (int i = 0; i < instCount; i++)
				{
					Node node = inst[i];
					if (node is FinalNode)
					{
						FinalNode finalNode = (FinalNode) node;
						if (finalNode.dataString().Equals("while") || finalNode.dataString().Equals("if") || finalNode.dataString().Equals("("))
						{
							continue;
						}
						else if (finalNode.dataString().Equals(")"))
						{
							break;
						}
					}
					else
					{
						conditionalExpressions.Add(node);
					}
				}
    
				return conditionalExpressions;
			}
		}

		public virtual object getRealValue(ActivationFrame frame)
		{
			return "";
		}
	}

}