using System;

namespace DrEAM.nodes
{

	/// <summary>
	/// Created by donreamey on 3/22/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class KeywordNode extends NodeImpl
	public class KeywordNode : NodeImpl
	{
		public override NodeType Type
		{
			get
			{
				return NodeType.KeywordType;
			}
		}

		public override IntegralType IntegralType
		{
			get
			{
				FinalNode finalNode = (FinalNode) this.Instructions[0];
    
				switch (finalNode.dataString())
				{
					case "int":
						return IntegralType.jinteger;
					case "string":
						return IntegralType.jstring;
					case "double":
						return IntegralType.jdouble;
					case "float":
						return IntegralType.jfloat;
					case "long":
						return IntegralType.jlong;
					case "class":
						return IntegralType.juserDefined;
					default:
						throw new Exception("no type found");
				}
			}
		}
	}

}