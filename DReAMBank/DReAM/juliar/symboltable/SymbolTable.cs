using System.Collections.Generic;

namespace DrEAM.symboltable
{

	using Node = com.juliar.nodes.Node;
	using UserDefinedTypeNode = com.juliar.nodes.UserDefinedTypeNode;
	using UserDefinedTypeVariableDeclNode = com.juliar.nodes.UserDefinedTypeVariableDeclNode;
	using VariableNode = com.juliar.nodes.VariableNode;
	using Visitor = com.juliar.vistor.Visitor;

	/// <summary>
	/// Created by donreamey on 1/9/17.
	/// 
	/// root
	///   |
	///    - main
	///   |  |
	///   |  |- z1
	///   |  |
	///   |  |- X1
	///   |
	///    - test1
	///   |  |
	///   |  |- s
	///   |
	///    - foo1
	///   |
	///    - foo2
	/// </summary>
	public class SymbolTable
	{
		private const string IDENTIFIERTXT = "Identifier %s already exist";
		private Dictionary<string, SymbolTableNode> scopeHash = new Dictionary<string, SymbolTableNode>();
		private static SymbolTable symbolTable;
		private static Deque<string> currentScope = new ArrayDeque<string>();
		private Visitor visitor;

		public static SymbolTable createSymbolTable(Visitor v)
		{
			if (symbolTable == null)
			{
				symbolTable = new SymbolTable(v);
			}
			return symbolTable;
		}

		public static void clearSymbolTable()
		{
			currentScope.Empty;
			symbolTable = null;
		}

		private SymbolTable()
		{
		}

		private SymbolTable(Visitor v) : this()
		{
			visitor = v;
		}

		public virtual void addLevel(string level)
		{
			SymbolTableNode node = new SymbolTableNode(this);
			node.levelNode = level;

			if (scopeHash.ContainsKey(level))
			{
				visitor.addError(string.format(IDENTIFIERTXT, level));
			}
			else
			{
				currentScope.push(level);
				scopeHash[level] = node;
			}
		}

		public virtual void popScope()
		{
			currentScope.pop();
		}

		private void addChildVariable(VariableNode child)
		{
			SymbolTableNode node = scopeHash[currentScope.peek()];
			int count = 0;

			foreach (Node childNode in node.children)
			{
				if (child.variableName.Equals(((VariableNode) childNode).variableName))
				{
					count++;
				}
			}

			if (count > 0)
			{
				visitor.addError(string.format(IDENTIFIERTXT, child.variableName));
				return;
			}

			scopeHash[currentScope.peek()].Children.Add(child);
		}

		private void addChildUserDefined(UserDefinedTypeNode child)
		{
			SymbolTableNode node = scopeHash[currentScope.peek()];
			int count = 0;

			foreach (Node childNode in node.children)
			{
				if (child.TypeName.Equals(((UserDefinedTypeNode)childNode).TypeName))
				{
					count++;
				}
			}

			if (count > 0)
			{
				visitor.addError(string.format(IDENTIFIERTXT, child.TypeName));
				return;
			}

			scopeHash[currentScope.peek()].Children.Add(child);
		}

		public virtual void addChild(Node child)
		{
			if (child is VariableNode)
			{
				addChildVariable((VariableNode) child);
			}
			else if (child is UserDefinedTypeNode)
			{
				addChildUserDefined((UserDefinedTypeNode) child);
			}
		}

		public virtual Node getNode(Node child)
		{
			Node returnNode = null;

			if (child is VariableNode)
			{

				Deque<string> tempScope = new ArrayDeque<string>();
				tempScope.push(currentScope.pop());

				while (!currentScope.Empty)
				{

					if (child is VariableNode)
					{

						SymbolTableNode node = scopeHash[currentScope.peek()];
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
						Optional<Node> nodeS = node.children.stream().filter(f => f is VariableNode).filter(f => ((VariableNode) f).variableName.Equals(((VariableNode) child).variableName)).findFirst();

						returnNode = nodeS.orElse(null);
					}
					else if (child is UserDefinedTypeNode)
					{
						SymbolTableNode node = scopeHash[currentScope.peek()];

//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
						Optional<Node> nodeS2 = node.children.stream().filter(f => f is UserDefinedTypeNode).filter(f => ((UserDefinedTypeNode) f).TypeName.Equals(((UserDefinedTypeNode) child).TypeName)).findFirst();

						returnNode = nodeS2.orElse(null);
					}

					if (returnNode != null)
					{
						break;
					}

				   while (!tempScope.Empty)
				   {
						currentScope.push(tempScope.pop());
				   }
				}
			}

			if (returnNode == null)
			{
				throw new IllegalStateException("unable to find variable -" + ((VariableNode)child).variableName + "in scope for reassignment");
			}

			return returnNode;
		}

		public virtual bool doesChildExistInHash(UserDefinedTypeVariableDeclNode parent, Node child)
		{
			SymbolTableNode t = scopeHash[parent.UserDefinedVariableTypeName];

			if (t != null)
			{
				foreach (Node childNode in t.children)
				{
					if (((VariableNode) childNode).variableName.Equals(((VariableNode) child).variableName))
					{
						return true;
					}
				}
			}

			return false;
		}

		public virtual bool doesChildExistAtScope(Node child)
		{
			Deque<string> tempScope = new ArrayDeque<string>();
			tempScope.push(currentScope.pop());
			bool doesExist = false;

			while (!currentScope.Empty)
			{
				if (child is VariableNode)
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					doesExist = scopeHash[tempScope.peek()].children.stream().filter(f => f is VariableNode).filter(t => ((VariableNode) t).variableName.Equals(((VariableNode) child).variableName)).count() == 1;
				}
				else if (child is UserDefinedTypeNode)
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					doesExist = scopeHash[tempScope.peek()].children.stream().filter(f => f is UserDefinedTypeNode).filter(t => ((VariableNode) t).variableName.Equals(((UserDefinedTypeNode) child).TypeName)).count() == 1;

				}
				if (doesExist)
				{
					break;
				}

				tempScope.push(currentScope.pop());
			}


			while (!tempScope.Empty)
			{
				currentScope.push(tempScope.pop());
			}

			return doesExist;
		}


		internal class SymbolTableNode
		{
			private readonly SymbolTable outerInstance;

			public SymbolTableNode(SymbolTable outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal string levelNode;
			internal IList<Node> children = new List<Node>();

			public virtual string LevelNode
			{
				get
				{
					return levelNode;
				}
				set
				{
					this.levelNode = value;
				}
			}


			public virtual IList<Node> Children
			{
				get
				{
					return children;
				}
				set
				{
					this.children = value;
				}
			}

		}
	}

}