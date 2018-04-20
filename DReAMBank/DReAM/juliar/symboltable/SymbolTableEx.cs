using System.Collections.Generic;

namespace DrEAM.symboltable
{


	/// <summary>
	/// Created by don on 4/8/17.
	/// </summary>
	public class SymbolTableEx
	{
		public string levelName;
		public IList<SymbolTableEx> scope = new List<SymbolTableEx>();
	}
}