using System.Collections.Generic;

namespace DrEAM.pal
{


	/// <summary>
	/// Created by AndreiM on 1/1/2017.
	/// </summary>
	public class PrimitivesMap
	{
		private static readonly IDictionary<string, string> map;
		static PrimitivesMap()
		{
			map = new Dictionary<>();
			map["fileOpen"] = "sysFileOpen";
			map["printLine"] = "sysPrintLine";
			map["printInt"] = "sys_print_int";
			map["printFloat"] = "sys_print_float";
			map["printDouble"] = "sys_print_double";
			map["printLong"] = "sys_print_long";
			map["fileWrite"] = "sysFileWrite";
			map["print"] = "sysPrint";
			map["available_memory"] = "sysAvailableMemory";
		}


		public static string getFunction(string name)
		{
			string current = map[name];

			if (current == null)
			{
				return name;
			}

			return current;
		}

		public static bool? isPrimitive(string name)
		{
			return map[name] != null;
		}

	}

}