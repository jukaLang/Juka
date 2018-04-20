using System;
using System.Diagnostics;

namespace DrEAM.nodes
{

	using JuliarLogger = DrEAM.errors.JuliarLogger;
	using TerminalNode = org.antlr.v4.runtime.tree.TerminalNode;

	/// <summary>
	/// Created by donreamey on 10/28/16.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class FinalNode extends NodeImpl
	public class FinalNode : NodeImpl
	{
		private string dataString_Renamed;
		[NonSerialized]
		private object @object;
		private NodeType nodeType = NodeType.FinalType;
		private sbyte[] instructionBytes;

		public FinalNode(TerminalNode data)
		{
			instructionBytes = data.Text.Bytes;
			if (data.Text != null)
			{
				dataString_Renamed = data.Text;
			}
			else if (data.Symbol.Text != null)
			{
				dataString_Renamed = data.Symbol.Text;
			}
		}

		public FinalNode()
		{
		}

		public override void addInst(Node instruction)
		{
			base.addInst(instruction);
		}

		public virtual object DataString
		{
			set
			{
				@object = value;
			}
		}

		public virtual sbyte[] FinalNodeBytes
		{
			get
			{
				return instructionBytes;
			}
		}

		public virtual T getBytesAstype<T>(Type asType)
		{
		   return asType.cast(instructionBytes);
		}

		public virtual string dataString()
		{
			if (@object == null && dataString_Renamed == null)
			{
				return "";
			}

			if (@object != null && dataString_Renamed == null)
			{
				return @object.ToString();
			}

			return dataString_Renamed;

		}

		public virtual object dataObject()
		{
			if (@object != null)
			{
				return @object;
			}

			return null;
		}

		public override IntegralType IntegralType
		{
			get
			{
				if (dataString_Renamed.StartsWith("\"", StringComparison.Ordinal) && dataString_Renamed.EndsWith("\"", StringComparison.Ordinal))
				{
					return IntegralType.jstring;
				}
    
				try
				{
					if (dataString_Renamed.Equals("true", StringComparison.CurrentCultureIgnoreCase) || dataString_Renamed.ToLower().EndsWith("false", StringComparison.Ordinal))
					{
						return IntegralType.jboolean;
					}
				}
				catch (Exception e)
				{
					JuliarLogger.log(e);
				}
				try
				{
					return IntegralType.jinteger;
				}
				catch (Exception e)
				{
					JuliarLogger.log(e);
				}
    
				try
				{
					return IntegralType.jdouble;
				}
				catch (Exception e)
				{
					JuliarLogger.log(e);
				}
    
				try
				{
					return IntegralType.jfloat;
				}
				catch (Exception e)
				{
					JuliarLogger.log(e);
				}
    
				try
				{
					return IntegralType.jlong;
				}
				catch (Exception e)
				{
					JuliarLogger.log(e);
				}
    
				return IntegralType.jobject;
			}
		}

		public override NodeType Type
		{
			get
			{
				dataString();
				if (dataString_Renamed == null)
				{
					return NodeType.FinalType;
				}
    
				switch (dataString_Renamed)
				{
					case "==":
						nodeType = NodeType.EqualEqualType;
						break;
					case "function":
						nodeType = NodeType.FunctionDeclType;
						break;
					case "<=":
					case "=>":
					case "<":
					case ">":
					default:
						Debug.Assert(false, "Type not implemented");
					break;
				}
    
				return nodeType;
			}
		}

	}

}