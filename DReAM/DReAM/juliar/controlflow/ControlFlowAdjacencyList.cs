using System;
using System.Collections.Generic;

namespace DrEAM.controlflow
{

	using JuliarLogger = DrEAM.errors.JuliarLogger;

	/// <summary>
	/// Created by Don on 2/1/2017.
	/// </summary>
	public class ControlFlowAdjacencyList
	{
		private IList<ControlFlowNode> list = new List<ControlFlowNode>();
		private LinkedBlockingDeque<ControlFlowNode> queue = new LinkedBlockingDeque<ControlFlowNode>();

		public virtual void addNode(string caller, string callee)
		{
			ControlFlowNode parent = findNode(caller);
			ControlFlowNode child = findNode(callee);
			addNext(parent, child);
		}

		public virtual void walkGraph()
		{
			ControlFlowNode main = findNode("main");
			walkGraph(main);
		}

		private void walkGraph(ControlFlowNode main)
		{
			try
			{
				main.visited = true;
				queue.addFirst(main.next);
				while (!queue.Empty)
				{
					ControlFlowNode g = queue.removeLast();
					if (!g.visited)
					{
						g.visited = true;
					}
					while (g != null)
					{
						if (!g.visited)
						{
							queue.addFirst(g);
						}
						g = g.next;
					}
				}
			}
			catch (Exception e)
			{
				JuliarLogger.log(e);
			}

		}

		private void addNext(ControlFlowNode p, ControlFlowNode n)
		{
			if (p.next == null)
			{
				p.next = n;
			}
			else if (p.next != n)
			{
				addNext(p.next, n);
			}
		}

		private ControlFlowNode findNode(string name)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			long count = list.stream().filter(f => f.functionName.Equals(name)).count();
			if (count == 0)
			{
				ControlFlowNode n = new ControlFlowNode(name);
				list.Add(n);
				return n;
			}

			foreach (ControlFlowNode node in list)
			{
				if (node.functionName.Equals(name))
				{
					return node;
				}
			}

			throw new Exception("cfa failed");
		}
	}

	internal class ControlFlowNode
	{
		public ControlFlowNode next;
		public string functionName;
		public bool visited = false;

		internal ControlFlowNode(string funcName)
		{
			functionName = funcName;
			next = null;
		}
	}
}