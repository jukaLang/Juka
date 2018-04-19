using System.Collections.Generic;

namespace DrEAM.nodes
{


	/// <summary>
	/// Created by don on 5/18/17.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class UserDefinedTypeNode extends NodeImpl
	public class UserDefinedTypeNode : NodeImpl
	{
		private string typeName;
		private string keyWordName;

		public IList<string> variableList = new List<string>();
		public IList<string> functionList = new List<string>();

		public UserDefinedTypeNode()
		{
		}

		public UserDefinedTypeNode(string typeName, string keyWordName)
		{
			this.typeName = typeName;
			this.keyWordName = keyWordName;
		}

		public virtual string TypeName
		{
			get
			{
				return ((FinalNode)instructions[1].Instructions[0]).dataString();
			}
		}

		public override NodeType Type
		{
			get
			{
				return null;
			}
		}

		public virtual VariableNode VariableNode
		{
			get
			{
				return (VariableNode)this.Instructions[2];
			}
		}

		public virtual FinalNode ObjectIdentifier
		{
			get
			{
				return (FinalNode)this.Instructions[0];
			}
		}

		public virtual FinalNode VariableIdentifer
		{
			get
			{
				VariableNode variableNode = (VariableNode)this.Instructions[2];
				return variableNode.FinalNode;
			}
		}

		public virtual string FullyQualifiedVariableName
		{
			get
			{
				FinalNode @object = ObjectIdentifier;
				FinalNode variableName = VariableIdentifer;
    
				return @object.dataString() + "::" + variableName.dataString();
			}
		}

		public virtual IList<string> AllVariableNames
		{
			get
			{
				StatementNode statementNode = (StatementNode)instructions[4];
				return statementNode.findAllVariablesInStatement();
			}
		}

	}

}