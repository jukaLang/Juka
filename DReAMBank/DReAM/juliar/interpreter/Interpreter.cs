using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DrEAM.interpreter
{

	using InstructionInvocation = DrEAM.codegenerator.InstructionInvocation;
	using JuliarLogger = DrEAM.errors.JuliarLogger;
	using com.juliar.nodes;

	/// <summary>
	/// Created by Don Reamey on 1/8/17.
	/// </summary>
	public class Interpreter
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			operatorPrecedenceTable = new operatorState(this);
		}

		private const string mainFunctionName = "main";
		private static ActivationFrameStack activationFrameStack = new ActivationFrameStack();
		private Stack<Node> returnValueStack = new Stack<Node>();
		private IDictionary<string, Node> functionNodeMap;
		private IDictionary<NodeType, Evaluate> functionMap = new Dictionary<NodeType, Evaluate>();
		public Stack<Node> operatorStack = new Stack<Node>();
		public Stack<Node> operandStack = new Stack<Node>();
		private operatorState operatorPrecedenceTable;


		private enum SrEval
		{
			Shift,
			Reduce,
			Error,
			End,
		}

		private class operatorState
		{
			private readonly Interpreter outerInstance;

			public operatorState(Interpreter outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public SrEval[][] shiftReduceOperationTable = new SrEval[][] {new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Reduce, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Reduce, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Reduce, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Reduce, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Reduce, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce,SrEval.Reduce, SrEval.Error, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Reduce, SrEval.Reduce, SrEval.Error, SrEval.Reduce, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Error, SrEval.Shift, SrEval.Error, SrEval.End, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Shift, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error}, new SrEval[] {SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Reduce, SrEval.Reduce}, new SrEval[] {SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Reduce, SrEval.Reduce}, new SrEval[] {SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Shift, SrEval.Reduce, SrEval.Error, SrEval.Error, SrEval.Error, SrEval.Reduce, SrEval.Reduce}};
		}

	/*
	    private class operatorBinding {
	        public
	    }
	*/

		public Interpreter(InstructionInvocation invocation)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			try
			{
				EvaluateAssignments.create(this);
				IList<Node> inst = invocation.InstructionList;
				activationFrameStack.push(new ActivationFrame("compliationUnit"));

				functionNodeMap = invocation.FunctionNodeMap;

				functionMap[NodeType.VariableReassignmentType] = (EvaluateAssignments::evalReassignment);
				functionMap[NodeType.AssignmentType] = (EvaluateAssignments::evalAssignment);
				functionMap[NodeType.PrimitiveType] = (EvaluatePrimitives::evalPrimitives);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.CompliationUnitType] = ((n, activationFrame, callback) => evalCompilationUnit());
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.AddType] = ((n, activationFrame, callback) => evalAdd());
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.CommandType] = ((n, activationFrame, callback) => evalCommand(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.SummationType] = ((n, activationFrame, callback) => evalSummation());
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.FunctionaCallType] = ((n, activationFrame, callback) => evalFunctionCall(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.FunctionDeclType] = ((n, activationFrame, callback) => evalFunctionDecl(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.VariableType] = ((n, activationFrame, callback) => evalActivationFrame(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.BinaryType] = ((n, activationFrame, callback) => evalBinaryNode(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.StatementType] = ((n, activationFrame, callback) => evalStatement(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.ExpressionType] = ((n, activationFrame, callback) => evalStatement(n));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.FinalType] = ((n, activationFrame, callback) => evalFinal());
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.EvaluatableType] = ((n, activationFrame, callback) => evaluateEvauatable(n, activationFrame, callback));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				functionMap[NodeType.UserDefinedFunctionReferenceType] = ((n, activationFrameStack, callback) => evalUserDefinedFunctionCall(n));
				functionMap[NodeType.AggregateType] = (this::evaluateAggregate);
				functionMap[NodeType.ReturnValueType] = (this::evalReturn);
				functionMap[NodeType.BooleanType] = (this::evalBooleanNode);
				functionMap[NodeType.BooleanOperatorType] = (this::evalBooleanOperator);
				functionMap[NodeType.IfExprType] = (this::evalIfStatement);
				functionMap[NodeType.WhileExpressionType] = (this::evalWhileExpression);
				functionMap[NodeType.VariableDeclarationType] = (this::evalVariableDeclration);

				//functionMap.put(NodeType.VariableDeclarationType, (n-> eval(n)));
				//functionMap.put(NodeType.ReturnValueType            , (n-> evalReassignment(n)      ));
				execute(inst);
			}
			catch (Exception ex)
			{
				JuliarLogger.log(ex);
			}
		}

		private bool? start = false;

		public virtual IList<Node> execute(IList<Node> instructions)
		{
			try
			{
				foreach (Node currentNode in instructions)
				{
					NodeType nodeType = currentNode.Type;
					if (functionMap.ContainsKey(nodeType))
					{
						Evaluate evaluate = functionMap[nodeType];
						if (evaluate != null)
						{
							IList<Node> instructionsToExecute = evaluate.evaluate(currentNode, activationFrameStack.peek(), this);
							if (instructionsToExecute != null && instructionsToExecute.Count > 0)
							{
								execute(instructionsToExecute);
							}
						}
						else
						{
							evalNull();
						}
					}
				}
			}
			catch (Exception ex)
			{
				JuliarLogger.log(ex);
			}

			return new List<>();
		}

		public virtual void evaluateExpressionStack()
		{
			Node @operator = null;
			if (operatorStack.Count == 0)
			{
				throw new Exception("not operators on stack");
			}

			@operator = operatorStack.Pop();

			//TODO ensure the type is accurate.
			//assert false : "this needs to fail until i fix it";
			if (@operator.Type.Equals(NodeType.EqualEqualType) && operandStack.Count >= 2)
			{
				NodeImpl r = (NodeImpl) operandStack.Pop();
				NodeImpl l = (NodeImpl) operandStack.Pop();

				ActivationFrame frame = activationFrameStack.peek();

				object rReal = r.getRealValue(frame);
				object lreal = l.getRealValue(frame);

				bool equalEqualEval = rReal.Equals(lreal);
				BooleanNode booleanNode = new BooleanNode();
				FinalNode finalNode = new FinalNode();
				finalNode.DataString = equalEqualEval;
				booleanNode.addInst(finalNode);

				operandStack.Push(booleanNode);
			}
		}

		public virtual void pushOperandStack(Node node)
		{
			operandStack.Push(node);
		}

		public virtual void pushOperatorStack(Node node)
		{
			if (operatorStack.Count == 0)
			{
				operatorStack.Push(node);
			}
			else
			{
				FinalNode currentOperand = (FinalNode) operatorStack.Peek();
				int currentNodePrecedence = getOperatorPrecedenceValue(currentOperand);
				int newNodePrecedence = getOperatorPrecedenceValue((FinalNode) node);

				SrEval eval = operatorPrecedenceTable.shiftReduceOperationTable[currentNodePrecedence][newNodePrecedence];

				if (eval == SrEval.Shift)
				{
					operatorStack.Push(node);
					return;
				}
				if (eval == SrEval.Reduce)
				{
					if (operandStack.Count >= 2)
					{
						Node rValue = operandStack.Pop();
						Node lValue = operandStack.Pop();
						Node operation = operatorStack.Pop();

						evalBooleanOperation(operation, lValue, rValue);
						// Need to Evaluate the operation.
						// Create a new node?
					}
				}
			}
		}

		private bool? evalBooleanOperation(Node operation, Node lValue, Node rValue)
		{
			if (operation is FinalNode)
			{
				string opType = ((FinalNode) operation).dataString();
				string lvalueString;

				switch (opType)
				{
					case "==":

						if (lValue is FinalNode)
						{
							lvalueString = ((FinalNode) lValue).dataString();
						}
						else if (lValue is LiteralNode)
						{
							LiteralNode literalNode = (LiteralNode)lValue.Instructions[0];
							// lvalueString = (LiteralNode)literalNode.getInstructions().get( 0 );
						}

						break;
					case ">=":
						break;
					case "<=":
						break;
					case "&&":
						if (lValue is FinalNode)
						{
							lvalueString = ((FinalNode) lValue).dataString();
						}
						else if (lValue is LiteralNode)
						{
							LiteralNode literalNode = (LiteralNode)lValue.Instructions[0];
						   // lvalueString = (LiteralNode)literalNode.getInstructions().get( 0 );
						}

						//if ( )
						break;
					case "||":
						break;

				}
			}

			return false;
		}

		private int getOperatorPrecedenceValue(FinalNode node)
		{
			int precedence = -1;
			string dataString = node.dataString();
			switch (dataString)
			{
				case "*":
				case "/":
				case "%":
					precedence = 7;
					break;
				case "+":
				case "-":
					precedence = 0;
					break;
				case "(":
					precedence = 9;
					break;
				case ")":
					precedence = 10;
					break;
				case "==":
					precedence = 13;
					break;
				case "&&":
					precedence = 14;
					break;
				case "||":
					precedence = 15;
					break;
			}

			return precedence;
		}

		private IList<Node> evalCompilationUnit()
		{
			foreach (KeyValuePair<string, Node> entry in functionNodeMap)
			{
				if (entry.Key.Equals(mainFunctionName))
				{

					ActivationFrame frame = new ActivationFrame();
					frame.frameName = mainFunctionName;
					activationFrameStack.push(frame);
					execute(entry.Value.Instructions);
					activationFrameStack.pop();

					break;
				}
			}

			return new List<>();
		}

		private IList<Node> evalStatement(Node node)
		{
			execute(node.Instructions);
			return new List<>();
		}

		private IList<Node> evalActivationFrame(Node node)
		{
			ActivationFrame frame = activationFrameStack.peek();
			frame.variableSet[((VariableNode)node).variableName] = node;
			return new List<>();
		}

		private IList<Node> evalSummation()
		{
			return new List<>();
		}

		private IList<Node> evalCommand(Node node)
		{
			IList<Node> slotList = new List<Node>();
			slotList.Add(node.Instructions[0]);
			return slotList;
		}

		private IList<Node> evaluateBreak(Node node, ActivationFrame frame)
		{
			frame.pushReturnNode(node);
			return new List<>();
		}

		private IList<Node> evalAdd()
		{
			return new List<>();
		}

		private IList<Node> evaluateEvauatable(Node node, ActivationFrame frame, Interpreter interpreter)
		{
				foreach (Node n in node.Instructions)
				{
						if (n is FinalNode)
						{
							string data = ((FinalNode) n).dataString();
							switch (data)
							{
								case "+":
								case "-":
								case "*":
								case "/":
								case ">=":
								case "<=":
								case "&&":
								case "==":
									interpreter.pushOperatorStack(n);
									break;
								default:
									interpreter.pushOperandStack(n);
								break;
							}
						}

						if (n is LiteralNode)
						{
							evaluateEvauatable(n, frame, interpreter);
						}

						if (n is FunctionCallNode)
						{

						}

						if (n is VariableNode)
						{
							string s = ((VariableNode)n).VariableName;
							interpreter.pushOperandStack(n);
						}

						if (n is BinaryNode)
						{
							evaluateEvauatable(n, frame, interpreter);
						}
				}
	/*
	            }
	
	            if ( shouldEvaluate ) {
	                interpreter.evaluateExpressionStack();
	            }
	
	        }
	*/
			return new List<>();
		}

		private IList<Node> evalNull()
		{
			Debug.Assert(true, "called evalNull");
			return new List<>();
		}

		private IList<Node> evalFinal()
		{
			return new List<>();
		}

		private IList<Node> evalReturn(Node oldnode, ActivationFrame frame, Interpreter callback)
		{
			return activationFrameStack.setupReturnValueOnStackFrame(oldnode);
		}

		private IList<Node> evalWhileExpression(Node node, ActivationFrame frame, Interpreter callback)
		{
			IList<Node> instructionList = ((NodeImpl)node).ConditionalExpressions;

			Node expressionNode = instructionList[0];

			if (expressionNode is EvaluatableNode)
			{
				expressionNode.EvaluateNode(frame, callback);
			}

			/*
			BooleanOperatorNode booleanNode = (BooleanOperatorNode) instructionList.get( 0 );
	
			evalBooleanNode(booleanNode, frame, callback);
	
			Node boolEvalResult = null;
	
			if (frame.peekReturnNode() != null) {
			    boolEvalResult = frame.popNode();
	
			    FinalNode finalNode = (FinalNode) boolEvalResult.getInstructions().get(0);
			    Boolean executeTrue = Boolean.parseBoolean(finalNode.dataString());
	
			    if (executeTrue) {
			        Boolean breakStatement = false;
			        while(true) {
			            for (int expressionCount = 0; expressionCount < trueExpressions.size(); expressionCount++) {
			                List<Node> currentExpressionInWhileBody = new ArrayList<>();
			                Node currentNode = trueExpressions.get( expressionCount );
			                currentExpressionInWhileBody.add( currentNode );
	
			                if (currentNode instanceof StatementNode && currentNode.getInstructions().get(0).getInstructions().get(0) instanceof BreakExprNode){
			                    breakStatement = true;
			                    break;
			                }
	
			                execute(currentExpressionInWhileBody);
	
			                if (frame.peekReturnNode() instanceof BreakExprNode){
			                    breakStatement = true;
			                    break;
			                }
			            }
	
			            if (breakStatement) {
			                frame.pushReturnNode( null );
			                break;
			            }
	
			            // re-evaluateExpression the loop condtion
			            evalBooleanNode(booleanNode, frame, callback);
			            boolEvalResult = frame.popNode();
			            finalNode = (FinalNode) boolEvalResult.getInstructions().get(0);
	
			            //assert finalNode.dataString().equalsIgnoreCase( "true ") || finalNode.dataString().equalsIgnoreCase( "false" ) : "A boolean value was not returned";
	
			            if (!(Boolean.parseBoolean( finalNode.dataString() ))) {
			                break;
			            }
			        }
			    }
			}*/

			return new List<>();
		}



		private BooleanNode getBooleanExpressionNode(IList<Node> instructionList, int size, IList<Node> trueExpressions)
		{
			BooleanNode booleanNode = null;
			for (int i = 0; i < size; i++)
			{
				Node current = instructionList[i];

				if (current is BooleanNode)
				{
					booleanNode = (BooleanNode) current;
					continue;
				}

				if (current is StatementNode)
				{
					trueExpressions.Add(current);
				}
			}
			return booleanNode;
		}

		private IList<Node> evalIfStatement(Node node, ActivationFrame frame, Interpreter callback)
		{
			IList<Node> instructionList = node.Instructions;
			int size = instructionList.Count;


			IList<Node> trueExpressions = new List<Node>();
			BooleanNode booleanNode = getBooleanExpressionNode(instructionList, size, trueExpressions);

			if (booleanNode != null && booleanNode.Instructions.Count == 1)
			{
				FinalNode finalNode = (FinalNode)booleanNode.Instructions[0];
				if (finalNode.IntegralType == IntegralType.jboolean)
				{
					bool? @bool = Convert.ToBoolean(finalNode.dataString());
					if (@bool)
					{
						return trueExpressions;
					}
				}
			}
			else if (booleanNode != null && booleanNode.Instructions.Count > 0)
			{
				Node currentValue = frame.popNode();
				frame.pushReturnNode(null);
				bool? booleanResult = false;
				evalBooleanNode(booleanNode, frame, callback);

				if (frame.peekReturnNode() != null && frame.peekReturnNode() is BooleanNode)
				{
					Node booleanEvalReturnNode = frame.popNode();
					FinalNode result = getFinalNodeFromAnyNode(booleanEvalReturnNode);
					booleanResult = Convert.ToBoolean(result.dataString());
				}

				// TODO - FINISH THIS
				if (booleanResult && frame.peekReturnNode() != null)
				{
					Node returnNode = frame.popNode();
					if (returnNode is BreakExprNode)
					{
						IList<Node> returnList = new List<Node>();
						returnList.Add(returnNode);
						return new List<>();
					}
				}

				frame.pushReturnNode(currentValue);
			}

			return new List<>();
		}

		private IList<Node> evalFunctionDecl(Node node)
		{
			return new List<>();
		}

		private IList<Node> evalBooleanNode(Node node, ActivationFrame frame, Interpreter callback)
		{
			Node variableType = node.Instructions[0];

			if (node is BooleanOperatorNode)
			{
		  //      ((BooleanOperatorNode)node).evaluateExpression( frame , this);
			}

			try
			{
				Node lvalue = null;
				if (variableType is VariableNode)
				{
					string variableName = ((VariableNode) node.Instructions[0]).variableName;
					lvalue = frame.variableSet[variableName];

				}
				else
				{
					lvalue = variableType;
				}

				bool isEqualEqual;
				Node rvalue = null;
				// This is ugly code. Need to find a better way to
				// handle these cases.
				// Multiple ifs will only cause confusion.
				FinalNode updatedLvalue = null;
	/*
	            if (node.getInstructions().size() == 1) {
	                //lvalue must be a single boolean expression
	                if (lvalue instanceof FinalNode) {
	                    BooleanNode booleanNode = new BooleanNode();
	                    booleanNode.addInst(lvalue);
	                    frame.pushReturnNode( booleanNode );
	                    return new ArrayList<>();
	                }
	
	                if (lvalue instanceof PrimitiveNode) {
	                    updatedLvalue = (FinalNode) lvalue.getInstructions().get(0);
	                }
	
	            } else if (node.getInstructions().size() > 1) {
	                //if (booleanOperatorNode.getInstructions().get(0) instanceof EqualEqualSignNode) {
	                    //isEqualEqual;
	                //}
	                rvalue = node.getInstructions().get(2);
	                FinalNode updatedRvalue = null;
	                if (rvalue != null && rvalue instanceof PrimitiveNode) {
	                    updatedRvalue = (FinalNode) rvalue.getInstructions().get(0);
	                }
	
	                if (updatedLvalue != null) {
	                    lvalue = updatedLvalue;
	                }
	
	                if (updatedRvalue != null) {
	                    rvalue = updatedRvalue;
	                }
	
	                isEqualEqual = getFinalNodeFromAnyNode( lvalue) .dataString().equals( getFinalNodeFromAnyNode(rvalue).dataString());
	                //else if (booleanOperatorNode.getInstructions().get(0) instanceof  )
	                FinalNode finalNode = new FinalNode();
	                finalNode.setDataString(isEqualEqual);
	
	                BooleanNode booleanNode = new BooleanNode();
	                booleanNode.addInst(finalNode);
	
	                frame.pushReturnNode( booleanNode );
	                return new ArrayList<>();
	            }
	            */
			}
			catch (Exception ex)
			{
				JuliarLogger.log(ex.Message);
			}

			return new List<>();
		}

		private IList<Node> evalVariableDeclration(Node node, ActivationFrame frame, Interpreter callback)
		{
			if (node.Instructions.Count == 2)
			{
				VariableNode variableNode = (VariableNode) node.Instructions[1];
				frame.variableSet[variableNode.variableName] = variableNode;

			}
			else if (node.Instructions.Count > 2)
			{

				IList<Node> returnValue = EvaluateAssignments.evalVariableDeclWithAssignment(node, activationFrameStack, mainFunctionName, functionNodeMap);
				if (returnValue.Count > 0)
				{
					execute(returnValue);
				}
			}

			return new List<>();
		}

		private bool? doesExpressionContainFunctionCall(Node n)
		{
			foreach (Node expr in n.Instructions)
			{
				if (expr is FunctionCallNode)
				{
					return true;
				}
			}

			return false;
		}

		public virtual FinalNode getFinalNodeFromAnyNode(Node node)
		{
			if (node is FinalNode)
			{
				return (FinalNode)node;
			}

			if (node.Instructions.Count == 1)
			{
				return getFinalNodeFromAnyNode(node.Instructions[0]);
			}

			throw new Exception("not able to find a final node");
		}

		private bool isEqual(Node left, Node right)
		{
			return left.Equals(right);
		}

		private bool isLessThan(Node left, Node right)
		{
			return false; //left < right;
		}

		private bool isGreaterThan(Node left, Node right)
		{
			return false;
		}

		private IList<Node> evalBooleanOperator(Node node, ActivationFrame frame, Interpreter callback)
		{
			return new List<>();
		}

		private IList<Node> evalUserDefinedFunctionCall(Node n)
		{
			return EvaluateFunctionsCalls.evalUserDefinedFunctionCall(n, this);
		}

		private IList<Node> evalFunctionCall(Node node)
		{
			return EvaluateFunctionsCalls.evalFunctionCall(node, activationFrameStack, mainFunctionName, functionNodeMap, this);
		}


		private IList<Node> AggregateNode(Node node)
		{
			IList<IntegralTypeNode> integralTypeNodes = ((AggregateNode)node).data();
			int addCount = integralTypeNodes.Count - 1;
			//TODO Different Primitive Types //add
			foreach (IntegralTypeNode integralTypeNode in integralTypeNodes)
			{
				integralTypeNode(integralTypeNode);
			}
			for (int i = 0;i < addCount;i++)
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				binaryOperation((a,b) => a + b); //check
			}
			return new List<>();
		}

		private IList<Node> binaryNode(string variableName, Node node)
		{
			evalBinaryNode(node);
			ActivationFrame frame = activationFrameStack.peek();
			VariableNode v = (VariableNode) frame.variableSet[variableName];

			v.IntegralTypeNode = (IntegralTypeNode)frame.operandStack.Pop();
			return new List<>();
		}

		private IList<Node> evaluateAggregate(Node node, ActivationFrame frame, Interpreter callback)
		{
			SummationType summationType = (SummationType) node.Instructions[0];
			IList<Node> list = node.Instructions;

			int size = list.Count;
			int sum = 0;

			IList<VariableNode> listOfVariableNodes = new List<VariableNode>();

			for (int i = 1; i < size; i++)
			{
				if (node.Instructions[i] is VariableNode)
				{
					listOfVariableNodes.Add((VariableNode) node.Instructions[i]);
				}
				else
				{
					string value = ((FinalNode) list[i].Instructions[0]).dataString();
					sum += Convert.ToInt32(value);
				}
			}

			if (listOfVariableNodes.Count > 0)
			{
				sum += aggregateVariable(listOfVariableNodes, frame);
			}

			FinalNode returnNode = new FinalNode();
			returnNode.DataString = sum;
			frame.pushReturnNode(returnNode);

			return new List<>();
		}

		private int aggregateVariable(IList<VariableNode> variableNodeList, ActivationFrame frame)
		{
			int sum = 0;
			for (int i = 0; i < variableNodeList.Count; i++)
			{

				Node v = frame.variableSet[variableNodeList[i].variableName];
				FinalNode finalNode = null;

				if (v is FinalNode && v.Instructions.Count == 0)
				{
					finalNode = (FinalNode)v;
				}

				if (v is PrimitiveNode)
				{
					PrimitiveNode primitiveNode = (PrimitiveNode) v;
					finalNode = (FinalNode)primitiveNode.Instructions[0];
				}

				if (finalNode != null)
				{
					sum += Convert.ToInt32(finalNode.dataString());
				}
			}

			return sum;
		}


		private IList<Node> evalBinaryNode(Node node)
		{
			BinaryNode bn = (BinaryNode) node;
			/*
			String operation = bn.operation().name();
	
			Object ol = bn.left();
			Object or = bn.right();
	
			if (ol instanceof IntegralTypeNode) {
			    integralTypeNode((IntegralTypeNode) ol);
			}
	
			if (or instanceof IntegralTypeNode) {
			    integralTypeNode((IntegralTypeNode) or);
			}
	
			switch (operation.toLowerCase()) {
			    case "+":
			    case "add":
			        binaryOperation( (a, b) -> a + b );
			        break;
			    case "-":
			    case "subtract":
			        binaryOperation( (a, b) -> a - b );
			        break;
			    case "*":
			    case "multiply":
			        binaryOperation( (a, b) -> a * b );
			        break;
			    case "/":
			    case "divide":
			        binaryOperation( (a, b) -> a / b );
			        break;
			    case "%":
			    case "modulo":
			        binaryOperation( (a,b) -> a % b);
			        break;
			    default:
			        assert true;
			        break;
			}
			*/
			return new List<>();
		}

		private IList<Node> binaryOperation(IntegerMath integerMath)
		{
			//ActivationFrame frame = activationFrameStack.peek();
		 //   String data1 = ((IntegralTypeNode) frame.operandStack.pop()).getIntegralValue();
		  //  int v1 = Integer.decode(data1).intValue();


		 //   String data2 = ((IntegralTypeNode) frame.operandStack.pop()).getIntegralValue();
		 //   int v2 = Integer.decode(data2).intValue();

		 //   String sum = new Integer(integerMath.operation(v2, v1)).toString();

			//TODO - NEED to FIX THIS.
			//frame.operandStack.push(new IntegralTypeNode(sum, IntegralType.jinteger));
			return new List<>();
		}

		private void integralTypeNode(IntegralTypeNode itn)
		{
			ActivationFrame frame = activationFrameStack.peek();
			frame.operandStack.Push(itn);
		}

		internal interface IntegerMath
		{
			int operation(int a, int b);
		}

		/*
		interface Evaluate {
		    List<Node> evaluateExpression(Node node, ActivationFrame frame);
		}
		*/
	}

}