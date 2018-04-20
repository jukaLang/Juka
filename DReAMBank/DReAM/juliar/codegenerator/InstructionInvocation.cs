using System;
using System.Collections.Generic;

namespace DrEAM.codegenerator
{

	using Node = DrEAM.nodes.Node;


	/// <summary>
	/// Created by Don on 1/12/2017.
	/// </summary>
	[Serializable]
	public class InstructionInvocation
	{
		private const long serialVersionUID = 321323213;
		private IList<Node> instructionList = new List<Node>();
		private IDictionary<string, Node> functionNodeMap = new Dictionary<string, Node>();

		public InstructionInvocation(IList<Node> inst, IDictionary<string, Node> functionMap)
		{
			instructionList = inst;
			functionNodeMap = functionMap;
		}

		public virtual IList<Node> InstructionList
		{
			get
			{
				return instructionList;
			}
		}

		public virtual IDictionary<string, Node> FunctionNodeMap
		{
			get
			{
				return functionNodeMap;
			}
		}
	}

}