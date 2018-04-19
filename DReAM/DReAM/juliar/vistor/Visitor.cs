using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace DrEAM.vistor
{

	using InstructionInvocation = com.juliar.codegenerator.InstructionInvocation;
	using JuliarLogger = com.juliar.errors.JuliarLogger;
	using com.juliar.nodes;
	using Primitives = com.juliar.pal.Primitives;
	using JuliarBaseVisitor = com.juliar.parser.JuliarBaseVisitor;
	using JuliarParser = com.juliar.parser.JuliarParser;
	using SymbolTable = com.juliar.symboltable.SymbolTable;
	using ParserRuleContext = org.antlr.v4.runtime.ParserRuleContext;
	using ParseTree = org.antlr.v4.runtime.tree.ParseTree;
	using TerminalNode = org.antlr.v4.runtime.tree.TerminalNode;


	/// <summary>
	/// Created by donreamey on 10/21/16.
	/// </summary>
	public class Visitor : DrEAMBaseVisitor<Node>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			symbolTable = SymbolTable.createSymbolTable(this);
		}

		private static int functionDeclCount = 0;
		private static int ifDeclCount = 0;
		private static int whileDeclCount = 0;
		private static int classDeclCount = 0;
		private IList<Node> instructionList = new List<Node>();
		private Dictionary<string, Node> functionNodeMap = new Dictionary<string, Node>();
		private Stack<Node> funcContextStack = new Stack<Node>();
		private Stack<string> callStack = new Stack<string>();
		private SymbolTable symbolTable;
		private ImportsInterface importsInterfaceCallback;
		private IList<string> errorList = new List<string>();
		private Dictionary<string, UserDefinedTypeNode> declaredClasses = new Dictionary<string, UserDefinedTypeNode>();
		private StringBuilder importBuffer = new StringBuilder();

		public virtual InstructionInvocation instructions()
		{
			return new InstructionInvocation(instructionList, functionNodeMap);
		}

		public virtual bool queryFunction(string functionName)
		{
			return functionNodeMap.Count > 0 && functionNodeMap.ContainsKey(functionName);
		}

		public Visitor(ImportsInterface cb, bool skip)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			importsInterfaceCallback = cb;
		}

		public override Node visitCompileUnit(JuliarParser.CompileUnitContext ctx)
		{

			CompliationUnitNode node = new CompliationUnitNode();
			try
			{
				string nodeName = node.ToString();
				callStack.Push(nodeName);
				symbolTable.addLevel(nodeName);

				new IterateOverContext(this, ctx, this, node);

				instructionList.Add(node);

				popScope(node.Type);
				//cfa.walkGraph();
				//symbolTable.dumpSymbolTable();
			}
			catch (Exception ex)
			{
				JuliarLogger.log(ex.Message);
			}
			return node;
		}

		private void popScope(NodeType nodeType)
		{
				switch (nodeType)
				{
					case com.juliar.nodes.NodeType.FunctionDeclType:
						functionDeclCount--;
						break;
					case com.juliar.nodes.NodeType.IfExprType:
						ifDeclCount--;
						break;
					case com.juliar.nodes.NodeType.WhileExpressionType:
						whileDeclCount--;
						break;
					case com.juliar.nodes.NodeType.UserDefinedDeclarationType:
						classDeclCount--;
						break;
					default:
						return;
				}

			symbolTable.popScope();
		}

		public override Node visitStatement(JuliarParser.StatementContext ctx)
		{
			StatementNode node = new StatementNode();
			new IterateOverContext(this, ctx, this, node);
			return node;
		}

		public override Node visitEndLine(JuliarParser.EndLineContext ctx)
		{
			FinalNode finalNode = new FinalNode();
			new IterateOverContext(this, ctx, this, finalNode);
			return finalNode;
		}

		//TODO need to refactor and combine vistAdd and visitSubtract
		public override Node visitAdd(JuliarParser.AddContext ctx)
		{
			return iterateWrapper(ctx, this, new AggregateNode());
		}

		public override Node visitSummation(JuliarParser.SummationContext ctx)
		{
			return iterateWrapper(ctx, this, new SummationType());
		}

		public override Node visitSubtract(JuliarParser.SubtractContext ctx)
		{
			string text = ctx.subtraction().Text;
			if ("subtract".Equals(text) || "-".Equals(text))
			{
				if (ctx.types().size() == 2)
				{
					BinaryNode node = new BinaryNode();
					try
					{
	/*
	                    Node n = node.makeNode(
	                                Operation.subtract,
	                                ctx.types(0).accept(this),
	                                ctx.types(1).accept(this));
	                    n.addInst( funcContextStack, n);
	*/
					}
					catch (Exception ex)
					{
						JuliarLogger.log(ex.Message,ex);
					}
				}

				if (ctx.types().size() > 2)
				{
					IList<IntegralTypeNode> data = new List<IntegralTypeNode>();

					for (int i = 0; i < ctx.types().size(); i++)
					{
						data.Add((IntegralTypeNode) ctx.types(i).accept(this));
					}
					AggregateNode aggregateNode = new AggregateNode(Operation.subtract, data);

					FunctionDeclNode functionDeclNode = (FunctionDeclNode) funcContextStack.Peek();
					functionDeclNode.addInst(aggregateNode);
				}
			}
			return null;
		}

		public override Node visitFunctionDeclaration(JuliarParser.FunctionDeclarationContext ctx)
		{
			string funcName = ctx.funcName().Text;
			FunctionDeclNode functionDeclNode = new FunctionDeclNode(funcName, new List<Node>());

			callStack.Push(funcName);
			symbolTable.addLevel(funcName + "_" + functionDeclCount++);

			new IterateOverContext(this, ctx, this, functionDeclNode);

			callStack.Pop();
			popScope(functionDeclNode.Type);

			functionNodeMap[funcName] = functionDeclNode;

			return functionDeclNode;
		}

		public override Node visitFunctionCall(JuliarParser.FunctionCallContext ctx)
		{
			FunctionCallNode node = new FunctionCallNode();
			new IterateOverContext(this, ctx, this, node);

			return node;
		}

		private Node handleBooleanOperatorNode(ParserRuleContext ctx)
		{
			BooleanOperatorNode booleanOperatorNode = new BooleanOperatorNode();
			iterateWithTryCatch(ctx, booleanOperatorNode);
			return booleanOperatorNode;
		}

		public override Node visitEqualityExpression(JuliarParser.EqualityExpressionContext ctx)
		{
			Node node = handleBooleanOperatorNode(ctx);
			return node;
		}

		public override Node visitParenthesizedExpression(JuliarParser.ParenthesizedExpressionContext ctx)
		{
			ParenthesizedExpressionNode node = new ParenthesizedExpressionNode();
			iterateWithTryCatch(ctx, node);
			return node;
		}

		public override Node visitTypes(JuliarParser.TypesContext ctx)
		{
			IntegralTypeNode integralTypeNode = new IntegralTypeNode();

			IterateOverContext context = new IterateOverContext(this);
			context.iterateOverChildren(ctx.primitiveTypes(), this, integralTypeNode);

			return integralTypeNode;
		}

		public override Node visitPrimitives(JuliarParser.PrimitivesContext ctx)
		{
			PrimitiveNode primitiveNode = new PrimitiveNode();
			IterateOverContext context = new IterateOverContext(this);
			context.iterateOverChildren(ctx, this, primitiveNode);
			return primitiveNode;
		}

		public override Node visitTerminal(TerminalNode node)
		{
			Node n = funcContextStack.Peek();
			if (n is FunctionDeclNode)
			{
				string name = ((FunctionDeclNode) n).FunctionName;
				if ("import".Equals(name))
				{
				   cacheImports(node.Text);

				}
			}

			string nodeText = node.Text;
			if (nodeText.Equals("()", StringComparison.CurrentCultureIgnoreCase) || nodeText.Equals("(", StringComparison.CurrentCultureIgnoreCase) || nodeText.Equals(")", StringComparison.CurrentCultureIgnoreCase) || nodeText.Equals("{", StringComparison.CurrentCultureIgnoreCase) || nodeText.Equals("}", StringComparison.CurrentCultureIgnoreCase) || nodeText.Equals(";", StringComparison.CurrentCultureIgnoreCase))
			{
				return new AnnotatedNode();
			}


			return new FinalNode(node);
		}

		public override Node visitBreakKeyWord(JuliarParser.BreakKeyWordContext ctx)
		{
			return iterateWrapper(ctx, this, new BreakExprNode());
		}

		public override Node visitAssignmentOperator(JuliarParser.AssignmentOperatorContext ctx)
		{
			return iterateWrapper(ctx, this, new EqualSignNode());
		}

		public override Node visitLiteral(JuliarParser.LiteralContext ctx)
		{
			return iterateWrapper(ctx, this, new LiteralNode());
		}

		public override Node visitVariableDeclarationExpression(JuliarParser.VariableDeclarationExpressionContext ctx)
		{
			VariableDeclarationNode node = new VariableDeclarationNode();
			iterateWrapper(ctx, this, node);

			if (node.Instructions.Count == 0 || node.Instructions.Count == 2)
			{
				// variable has been declared but will be set to null by default.
				node.addInst(new FinalNode());
			}

			return node;
		}

		public override Node visitVariableExpression(JuliarParser.VariableExpressionContext ctx)
		{
			return base.visitVariableExpression(ctx);
		}

		public override Node visitBinaryExpression(JuliarParser.BinaryExpressionContext ctx)
		{
			BinaryNode node = new BinaryNode();
			iterateWithTryCatch(ctx, node);
			return node;
		}

		public override Node visitAssignmentExpression(JuliarParser.AssignmentExpressionContext ctx)
		{
			AssignmentNode node = new AssignmentNode(null);

			IterateOverContext iterateOverContext = new IterateOverContextAnonymousInnerClassHelper(this);

			iterateOverContext.iterateOverChildren(ctx, this, node);
			return node;
		}

		private class IterateOverContextAnonymousInnerClassHelper : IterateOverContext
		{
			private readonly Visitor outerInstance;

			public IterateOverContextAnonymousInnerClassHelper(Visitor outerInstance) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void action(Node parent, Node child)
			{
				// Set the integral type of the expression for type checking the right hand
				// side of the expression during parsing and at runtime.
				if (child is VariableDeclarationNode)
				{
					VariableDeclarationNode variableDeclarationNode = (VariableDeclarationNode) child;
					parent.VariableTypeByIntegralType = variableDeclarationNode.IntegralType;
				}

				if (parent is AssignmentNode && parent.Instructions.Count >= 2 && parent.Instructions[0] is VariableDeclarationNode)
				{
					if (parent.Instructions[0].Instructions.get(0) is UserDefinedTypeNode)
					{
						parent.Instructions[0].Instructions.get(0).Instructions.get(2).IntegralType;
						return;
					}
					else
					{
						VariableNode variableNode = (VariableNode) parent.Instructions[0].Instructions.get(1);
						variableNode.IntegralType;
						/* if (parent.getIntegralType() != variableNode.getIntegralType()) {
						   throw new RuntimeException( "invalide types used in expressioin");
						}*/
					}
				}

				if (child is PrimitiveNode && parent.IntegralType != null && parent.IntegralType != child.IntegralType)
				{
					throw new Exception("invald types used in expression");
				}
			}
		}


		public override Node visitAssignmentOperatorExpression(JuliarParser.AssignmentOperatorExpressionContext ctx)
		{
			VariableReassignmentNode node = new VariableReassignmentNode();

			//TODO see if the node is in tye Symboltable and if it can be accessed. The original node with the type.
			iterateWrapper(ctx, this, node);
			if (symbolTable.doesChildExistAtScope(node.Instructions[0]))
			{
				VariableNode variableNode = (VariableNode)symbolTable.getNode(node.Instructions[0]);
				node.Instructions[0].VariableTypeByIntegralType = variableNode.IntegralType;
			}
			return node;
		}

		public override Node visitIfExpr(JuliarParser.IfExprContext ctx)
		{
			IfExprNode node = new IfExprNode();
			symbolTable.addLevel("if" + "_" + ifDeclCount++);

			iterateWrapper(ctx, this, node);

			symbolTable.popScope();

			return node;
		}

		public override Node visitNotOperator(JuliarParser.NotOperatorContext ctx)
		{
			return handleBooleanOperatorNode(ctx);
		}

		public override Node visitNumericTypes(JuliarParser.NumericTypesContext ctx)
		{
			return base.visitNumericTypes(ctx);
		}

		public override Node visitPrimitiveTypes(JuliarParser.PrimitiveTypesContext ctx)
		{
			PrimitiveNode node = new PrimitiveNode();
			new IterateOverContext(this, ctx, this, node);
			return node;
		}

		public override Node visitEqualsign(JuliarParser.EqualsignContext ctx)
		{
			return new EqualSignNode();
		}

		   public override Node visitExpression(JuliarParser.ExpressionContext ctx)
		   {
			ExpressionNode node = new ExpressionNode();
			new IterateOverContext(this, ctx, this, node);
			return node;
		   }

		public override Node visitVariableDeclaration(JuliarParser.VariableDeclarationContext ctx)
		{
			VariableDeclarationNode variableDeclarationNode = new VariableDeclarationNode();

			new IterateOverContext(this, ctx, this, variableDeclarationNode);
			VariableNode variableNode;

			if (variableDeclarationNode.Instructions[0] is UserDefinedTypeNode)
			{
				variableNode = (VariableNode) variableDeclarationNode.Instructions[0].Instructions[2];
				variableNode.VariableType = ctx.children.get(0).getChild(0).Text;
			}
			else if (variableDeclarationNode.Instructions.Count >= 2)
			{
				variableNode = (VariableNode) variableDeclarationNode.Instructions[1];
				variableNode.VariableType = ctx.children.get(0).Text;
			}

			return variableDeclarationNode;
		}

		public override Node visitKeywords(JuliarParser.KeywordsContext ctx)
		{
			KeywordNode keywordNode = new KeywordNode();
			new IterateOverContext(this, ctx, this, keywordNode);

			return keywordNode;
		}

		public override Node visitBooleanExpression(JuliarParser.BooleanExpressionContext ctx)
		{
			BooleanNode node = new BooleanNode();
			IterateOverContext iterateOverContext = new IterateOverContextAnonymousInnerClassHelper2(this, node);

			iterateOverContext.iterateOverChildren(ctx, this, node);

			node.determineBooleanComparisionType();

			return node;
		}

		private class IterateOverContextAnonymousInnerClassHelper2 : IterateOverContext
		{
			private readonly Visitor outerInstance;

			private com.juliar.nodes.BooleanNode node;

			public IterateOverContextAnonymousInnerClassHelper2(Visitor outerInstance, com.juliar.nodes.BooleanNode node) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				this.node = node;
			}

			public override void action(Node node)
			{
				base.action(node);
			}
		}

		public override Node visitCommand(JuliarParser.CommandContext ctx)
		{
			CommandNode commandNode = new CommandNode();
			new IterateOverContext(this, ctx, this, commandNode);
			return commandNode;
		}

		public override Node visitVariable(JuliarParser.VariableContext ctx)
		{
			Node iteratorNode;

			try
			{
				string variableName = "";

				if (ctx.ID() != null)
				{
					variableName = ctx.ID().Text;
				}

				VariableNode variableNode = new VariableNode(variableName);

				if (variableNode == null)
				{
					throw new Exception("unable to create a variable");
				}

				object[] funcStackArray = funcContextStack.ToArray();
				int length = funcStackArray.Length - 1;
				int index = length;

				for (; index >= 0; index--)
				{
					if (funcStackArray[index] is VariableDeclarationNode)
					{
						// We are creating the variable and adding it to the symbol table.
						// This will automatically throw an exception if creating a symbol with
						// same name at same scope.
						symbolTable.addChild(variableNode);
						break;
					}

					if (funcStackArray[index] is UserDefinedTypeNode)
					{
						Debug.Assert(true, "should not hit this");
						// TODO
						// user defined variables will need to be looked up in the class / variable map.
						break;
					}

					if (funcStackArray[index] is UserDefinedTypeVariableDeclNode)
					{
						UserDefinedTypeVariableDeclNode temp = (UserDefinedTypeVariableDeclNode) funcStackArray[index];
						variableNode.Parent = temp;
						if (!symbolTable.doesChildExistAtScope(variableNode))
						{
							symbolTable.addChild(variableNode);
						}
						break;
					}

					if (funcStackArray[index] is UserDefinedTypeVariableReference)
					{
						handleUserDefinedTypeVariableReference(variableName, funcStackArray[index]);
						break;
					}

					if (!symbolTable.doesChildExistAtScope(variableNode))
					{
						addError("The variable [" + variableName + "] is not declared at the scope");
					}
				}

				iteratorNode = iterateWrapper(ctx, this, variableNode);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return iteratorNode;
		}

		private void handleUserDefinedTypeVariableReference(string variableName, object functionStackObject)
		{
			UserDefinedTypeVariableReference tempRef = (UserDefinedTypeVariableReference) functionStackObject;
			string objectName = tempRef.ObjectName;

			VariableNode objectVariable = new VariableNode(objectName);
			UserDefinedTypeVariableDeclNode parent = null;
			if (symbolTable.doesChildExistAtScope(objectVariable))
			{
				Node localVar = symbolTable.getNode(objectVariable);
				if (localVar is VariableNode)
				{
					VariableNode localVariableNode = (VariableNode)localVar;

					if (localVariableNode.Parent is UserDefinedTypeVariableDeclNode)
					{
						parent = (UserDefinedTypeVariableDeclNode) localVariableNode.Parent;
					}
				}
			}

			if (parent == null)
			{
				throw new Exception(string.Format("The user defined variable {0} does not have an accessible parent", variableName));
			}

			string className = parent.UserDefinedVariableTypeName;

			if (declaredClasses.ContainsKey(className))
			{
				UserDefinedTypeNode userDefinedTypeNode = declaredClasses[className];

				if (!userDefinedTypeNode.AllVariableNames.Contains(variableName))
				{
					addError("the object [ " + className + "]   does not have a varible [" + variableName + "] defined as part of its instance");
				}

				VariableNode tempVariableNode = new VariableNode(variableName);

				if (!symbolTable.doesChildExistInHash(parent, tempVariableNode))
				{
					symbolTable.addChild(tempVariableNode);
				}
			}
		}

		public override Node visitReturnValue(JuliarParser.ReturnValueContext ctx)
		{
			ReturnValueNode node = new ReturnValueNode();

			new IterateOverContext(this, ctx, this, node);

			return node;
		}

		public override Node visitWhileExpression(JuliarParser.WhileExpressionContext ctx)
		{
			symbolTable.addLevel("while" + "_" + whileDeclCount++);

			WhileExprNode whileExprNode = new WhileExprNode();
			iterateWrapper(ctx, this, whileExprNode);

			symbolTable.popScope();
			return whileExprNode;
		}

		public virtual void addError(string error)
		{
			errorList.Add(error);
		}

		public virtual IList<string> ErrorList
		{
			get
			{
				return errorList;
			}
		}


		public override Node visitEvaluatable(JuliarParser.EvaluatableContext ctx)
		{
			Node node = new EvaluatableNode();
			iterateWrapper(ctx, this, node);
			return node;
		}

		public override Node visitUserDefinedTypeDecl(JuliarParser.UserDefinedTypeDeclContext ctx)
		{
			UserDefinedTypeNode userDefinedTypeNode = new UserDefinedTypeNode();

			symbolTable.addLevel(ctx.userDefinedTypeName().Text);

			iterateWrapper(ctx, this, userDefinedTypeNode);

			if (declaredClasses.ContainsKey(userDefinedTypeNode.TypeName))
			{
				throw new Exception("class " + userDefinedTypeNode.TypeName + "already exist at current scope");
			}

			declaredClasses[userDefinedTypeNode.TypeName] = userDefinedTypeNode;

			symbolTable.popScope();

			return userDefinedTypeNode;
		}

		public override Node visitUserDefinedTypeKeyWord(JuliarParser.UserDefinedTypeKeyWordContext ctx)
		{
			KeywordNode keywordNode = new KeywordNode();
			return iterateWithTryCatch(ctx, keywordNode);
		}

		public override Node visitUserDefinedTypeName(JuliarParser.UserDefinedTypeNameContext ctx)
		{
			UserDefinedTypeNameNode userDefinedTypeNameNode = new UserDefinedTypeNameNode();
			return iterateWithTryCatch(ctx, userDefinedTypeNameNode);
		}

		public override Node visitUserDefinedTypeFunctionReference(JuliarParser.UserDefinedTypeFunctionReferenceContext ctx)
		{
			UserDefinedTypeFunctionReferenceNode node = new UserDefinedTypeFunctionReferenceNode();
			funcContextStack.Push(node);
			iterateWithTryCatch(ctx, node);
			funcContextStack.Pop();

			return node;
		}

		private T iterateWithTryCatch<S, T>(S ctx, T node) where S : org.antlr.v4.runtime.ParserRuleContext where T : NodeImpl
		{
			try
			{
				iterateWrapper(ctx, this, node);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return node;
		}

		public override Node visitDeleteExpression(JuliarParser.DeleteExpressionContext ctx)
		{
			return base.visitDeleteExpression(ctx);
		}

	/*
	    @Override
	    public Node visitLogicalOrExpression(JuliarParser.LogicalOrExpressionContext ctx) {
	        return super.visitLogicalOrExpression(ctx);
	    }
	*/

		public override Node visitUserDefinedTypeVariableDecl(JuliarParser.UserDefinedTypeVariableDeclContext ctx)
		{
			UserDefinedTypeVariableDeclNode node = new UserDefinedTypeVariableDeclNode();
			return iterateWithTryCatch(ctx, node);
		}

		public override Node visitUserDefinedTypeResolutionOperator(JuliarParser.UserDefinedTypeResolutionOperatorContext ctx)
		{
			ResolutionNode resolutionNode = new ResolutionNode();
			return iterateWithTryCatch(ctx, resolutionNode);
		}

		public override Node visitUserDefinedTypeVariableReference(JuliarParser.UserDefinedTypeVariableReferenceContext ctx)
		{
			UserDefinedTypeVariableReference node = new UserDefinedTypeVariableReference();
			return iterateWithTryCatch(ctx, node);
		}


		private Node iterateWrapper(ParserRuleContext ctx, Visitor visitor, Node parent)
		{
			IterateOverContext it = new IterateOverContext(this);
			it.iterateOverChildren(ctx, visitor, parent);
			return parent;
		}

		private void cacheImports(string fileName)
		{
			StringBuilder builder = new StringBuilder();
			try
			{
					using (BufferedReader bufferedReader = new BufferedReader(new FileReader(Primitives.stripQuotes(fileName))))
					{
					string line = bufferedReader.readLine();
					while (line != null)
					{
						builder.Append(line);
						line = bufferedReader.readLine();
					}
					}
			}
			catch (IOException e)
			{
				JuliarLogger.log(e);
			}

			importBuffer.Append(builder);
			int currentLineNumber = 0;
			importsInterfaceCallback.createTempCallback(importBuffer.ToString(), currentLineNumber);
		}

		internal class IterateOverContext
		{
			private readonly Visitor outerInstance;


			public IterateOverContext(Visitor outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public IterateOverContext(Visitor outerInstance, ParserRuleContext ctx, Visitor visitor, Node parent) : this(outerInstance)
			{
				this.outerInstance = outerInstance;
				iterateOverChildren(ctx, visitor, parent);
			}


			public virtual void iterateOverChildren(ParserRuleContext ctx, Visitor visitor, Node parent)
			{
				if (ctx.children == null)
				{
					return;
				}
				outerInstance.funcContextStack.Push(parent);

				for (IEnumerator<ParseTree> pt = ctx.children.GetEnumerator(); pt.MoveNext();)
				{
					ParseTree parseTree = pt.Current;
					Node node = parseTree.accept(visitor);

					if (node is AnnotatedNode)
					{
						continue;
					}

					action(parent, node);
					action(node);

					parent.addInst(node);

					if (node is NodeImpl)
					{
						if (((NodeImpl)node).ParentNode() == null)
						{
							Console.WriteLine("this is null");
						}
					}

				}

				outerInstance.funcContextStack.Pop();
			}


			/*
			 this method will be overridden in implementation.
			 */
			public virtual void action(Node node)
			{
			/*
			empty body
			*/
			}

			public virtual void action(Node parent, Node child)
			{
			/*
			empty body
			*/
			}
		}
	}

}