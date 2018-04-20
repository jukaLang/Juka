using System.Collections.Generic;

namespace DrEAM.codegenerator
{
	using IntegralType = DrEAM.nodes.IntegralType;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static org.objectweb.asm.Opcodes.*;

	/// <summary>
	/// Created by AndreiM on 11/14/2016.
	/// </summary>
	public class CodeGeneratorMap
	{
		private static readonly IDictionary<string, IList<int?>> map;
		private CodeGeneratorMap()
		{
		}


		static CodeGeneratorMap()
		{
			map = new Dictionary<>();
			map["add"] = new List<>(IADD,LADD,FADD,DADD);
			map["sub"] = new List<>(ISUB,LSUB,FSUB,DSUB);
			map["multiply"] = new List<>(IMUL,LMUL,FMUL,DMUL);
			map["divide"] = new List<>(IDIV, LDIV,FDIV,DDIV);
			map["modulo"] = new List<>(IREM,LREM,FREM,DREM);
		}


		public static IDictionary<IntegralType, int?> generateMap(string instruction)
		{
			IList<int?> current = map[instruction];
			IDictionary<IntegralType, int?> hmap = new EnumMap<IntegralType, int?>(typeof(IntegralType));
			hmap[IntegralType.jinteger] = current[0];
			hmap[IntegralType.jlong] = current[1];
			hmap[IntegralType.jfloat] = current[2];
			hmap[IntegralType.jdouble] = current[3];
			return hmap;
		}

		public static IList<int?> generateList(string instruction)
		{
			return map[instruction];
		}
	}

}