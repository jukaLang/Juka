using System;
using System.Collections.Generic;

namespace DrEAM.codegenerator
{

	using JuliarLogger = DrEAM.errors.JuliarLogger;
	using DrEAM.nodes;
	using Primitives = DrEAM.pal.Primitives;
	using PrimitivesMap = DrEAM.pal.PrimitivesMap;
	using ClassWriter = org.objectweb.asm.ClassWriter;
	using MethodVisitor = org.objectweb.asm.MethodVisitor;
	using GeneratorAdapter = org.objectweb.asm.commons.GeneratorAdapter;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static org.objectweb.asm.Opcodes.*;


	/// <summary>
	/// Created by donreamey on 10/22/16.
	/// </summary>
	public class CodeGenerator
	{
		private bool isDebug = true;
		public CodeGenerator(bool debug)
		{
			isDebug = debug;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void generate(InstructionInvocation invocation, String outputfile) throws java.io.IOException
		public virtual void generate(InstructionInvocation invocation, string outputfile)
		{
			IList<Node> inst = invocation.InstructionList;
			generate(inst,outputfile);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void generate(java.util.List<Node> instructions, String outputfile) throws java.io.IOException
		public virtual void generate(IList<Node> instructions, string outputfile)
		{
			ClassWriter cw = new ClassWriter(0);
			cw.visit(V1_1, ACC_PUBLIC, outputfile, null, "java/lang/Object", null);

			// creates a MethodWriter for the (implicit) constructor
			MethodVisitor mw = cw.visitMethod(ACC_PUBLIC, "<init>", "()V", null, null);
			// pushes the 'this' variable
			mw.visitVarInsn(ALOAD, 0);
			// invokes the super class constructor
			mw.visitMethodInsn(INVOKESPECIAL, "java/lang/Object", "<init>", "()V", false);
			mw.visitInsn(RETURN);
			// this code uses a maximum of one stack element and one local variable
			mw.visitMaxs(1, 1);
			mw.visitEnd();


			// creates a MethodWriter for the 'main' method
			mw = cw.visitMethod(ACC_PUBLIC + ACC_STATIC, "main", "([Ljava/lang/String;)V", null, null);
			// pushes the 'out' field (of type PrintStream) of the System class
			mw.visitFieldInsn(GETSTATIC, "java/lang/System", "out","Ljava/io/PrintStream;");
			// pushes the "Hello Juliar Future" String constant
			mw.visitLdcInsn("Now Calling generated Juliar Methods!");
			// invokes the 'println' method (defined in the PrintStream class)
			mw.visitMethodInsn(INVOKEVIRTUAL, "java/io/PrintStream", "println","(Ljava/lang/String;)V", false);
			//mw.visitInsn(RETURN);
			// this code uses a maximum of two stack elements and two local
			// variables
			//mw.visitVarInsn(ALOAD,0);
			mw.visitMethodInsn(INVOKESTATIC, outputfile, "juliarMethod", "()V", false);
			mw.visitInsn(RETURN);

			mw.visitMaxs(2, 2);
			mw.visitEnd();

			mw = cw.visitMethod(ACC_PUBLIC + ACC_STATIC, "juliarMethod", "()V", null, null);

			int? stackSize = 0;
			GeneratorAdapter ga = new GeneratorAdapter(mw, ACC_PUBLIC + ACC_STATIC, "juliarMethod", "()V");
			evaluateExpressions(instructions, mw, ga, stackSize);



			mw.visitFieldInsn(GETSTATIC, "java/lang/System", "out","Ljava/io/PrintStream;");
			mw.visitLdcInsn("Instructions:" + instructions);
			mw.visitMethodInsn(INVOKEVIRTUAL, "java/io/PrintStream", "println","(Ljava/lang/String;)V", false);


			mw.visitInsn(RETURN);
			mw.visitMaxs(16, 6);
			mw.visitEnd();

			/*MethodVisitor foo = cw.visitMethod(ACC_PUBLIC + ACC_STATIC, "foo", "()V", null, null);
	
			GeneratorAdapter ga1 = new GeneratorAdapter(foo, ACC_PUBLIC + ACC_STATIC, "foo", "()V");
			foo.visitInsn(RETURN);
			foo.visitEnd();*/


			// gets the bytecode of the Example class, and loads it dynamically
			sbyte[] code = cw.toByteArray();

			//Create JAR output
			FileOutputStream fout = new FileOutputStream(outputfile + ".jar");

			Manifest manifest = new Manifest();
			manifest.MainAttributes.put(Attributes.Name.MANIFEST_VERSION, "1.0");
			manifest.MainAttributes.put(Attributes.Name.MAIN_CLASS, outputfile);

			JarOutputStream jarOut = new JarOutputStream(fout, manifest);

			try
			{
				jarOut.putNextEntry(new ZipEntry("com/juliar/pal/Primitives.class"));
				InputStream primitiveStream = typeof(Primitives).getResourceAsStream("Primitives.class");
				jarOut.write(getBytes(primitiveStream));
				jarOut.closeEntry();

				jarOut.putNextEntry(new ZipEntry(outputfile + ".class"));
				jarOut.write(code);
				jarOut.closeEntry();
			}
			catch (Exception e)
			{
				JuliarLogger.log(e);
			}
			jarOut.close();
			fout.close();


			/*
			List<String> Dependencies = SomeClass.getDependencies();
			FileOutputStream fout = new FileOutputStream(outputfile+".jar");
	
			JarOutputStream jarOut = new JarOutputStream(fout);
	
			//jarOut.putNextEntry(new ZipEntry("com/juliar/pal")); // Folders must end with "/".
			//jarOut.putNextEntry(new ZipEntry("com/juliar/pal/Primitives.class"));
			//jarOut.write(getBytes("com/juliar/pal/Primitives.class"));
			//jarOut.closeEntry();
	
			jarOut.putNextEntry(new ZipEntry(outputfile+".class"));
			jarOut.write(getBytes(outputfile+".class"));
			jarOut.closeEntry();
	
			for(String dependency : Dependencies){
			    int index = dependency.lastIndexOf( '/' );
			    jarOut.putNextEntry(new ZipEntry(dependency.substring( index ))); // Folders must end with "/".
			    jarOut.putNextEntry(new ZipEntry(dependency));
			    jarOut.write(getBytes(dependency));
			    jarOut.closeEntry();
			}
	
	
			jarOut.close();
	
			fout.close();*/
		}

		private MethodVisitor evaluateExpressions(IList<Node> instructions, MethodVisitor mw, GeneratorAdapter ga, int? stackSize)
		{
			JuliarLogger.log("x");
			foreach (Node instruction in instructions)
			{
				JuliarLogger.log("Instructions");
				if (instruction is FunctionDeclNode)
				{
					IList<Node> t = instruction.Instructions;
					evaluateExpressions(t, mw, ga, stackSize);
				}
				if (instruction is PrimitiveNode)
				{
					string function = ((PrimitiveNode) instruction).PrimitiveName.ToString();
					JuliarLogger.log(function);

					function = PrimitivesMap.getFunction(function);
					mw.visitLdcInsn(((PrimitiveNode) instruction).GetPrimitiveArgument.ToString());
					mw.visitIntInsn(ASTORE, 0);
					mw.visitIntInsn(ALOAD, 0);
					mw.visitMethodInsn(INVOKESTATIC, "com/juliar/pal/Primitives", function, "(Ljava/lang/String;)Ljava/lang/String;", false);


					if ("printLine".Equals(function))
					{
						function = "sysPrintLine";
						mw.visitLdcInsn(((PrimitiveNode) instruction).GetPrimitiveArgument.ToString());
						mw.visitIntInsn(ASTORE, 0);
						mw.visitIntInsn(ALOAD, 0);
						mw.visitMethodInsn(INVOKESTATIC, "com/juliar/pal/Primitives", function, "(Ljava/lang/String;)V", false);
					}

					if ("print".Equals(function))
					{
						function = "sysPrint";
						mw.visitLdcInsn(((PrimitiveNode) instruction).GetPrimitiveArgument.ToString());
						mw.visitIntInsn(ASTORE, 0);
						mw.visitIntInsn(ALOAD, 0);
						mw.visitMethodInsn(INVOKESTATIC, "com/juliar/pal/Primitives", function, "(Ljava/lang/String;)V", false);
					}

				}


				if (instruction is CompliationUnitNode)
				{
					//WWLogger.log(((CompliationUnitNode) instructions).getInstructions().toString());
					/*Logger.og(t.toString());
					 */

					IList<Node> t = instruction.Instructions;
					evaluateExpressions(t, mw, ga, stackSize);

					/*for (List<Node> n : t) {
					    Logger.log(n.getNodeName().toString());
					    Logger.log("HERE");
					    evaluateExpressions(n, mw, ga, stackSize);
					}*/
				}

				if (instruction is StatementNode)
				{
					IList<Node> t = instruction.Instructions;
					evaluateExpressions(t, mw, ga, stackSize);
				}

				if (instruction is BinaryNode)
				{
					/*
					JuliarLogger.log("BinaryNode");
					//Map<IntegralType,Integer> op = CodeGeneratorMap.generateMap(((BinaryNode)instruction).operation().toString());
	
					BinaryNode b = ((BinaryNode)instruction);
					if (isDebug) {
					    mw.visitFieldInsn(GETSTATIC, "java/lang/System", "out", "Ljava/io/PrintStream;");
					}
	
					IntegralType addType;
					IntegralTypeNode ln = (IntegralTypeNode)b.left();
					IntegralTypeNode rn = (IntegralTypeNode)b.right();
	
					if (ln.getIntegralType() == IntegralType.jdouble || rn.getIntegralType() == IntegralType.jdouble){
					    addType = IntegralType.jdouble;
					} else if (ln.getIntegralType() == IntegralType.jfloat || rn.getIntegralType() == IntegralType.jfloat){
					    addType =  IntegralType.jfloat;
					} else if (ln.getIntegralType() == IntegralType.jlong || rn.getIntegralType() == IntegralType.jlong){
					    addType = IntegralType.jlong;
					} else if (ln.getIntegralType() == IntegralType.jinteger || rn.getIntegralType() == IntegralType.jinteger){
					    addType = IntegralType.jinteger;
					} else{
					   addType = null;
					}
	
					pushIntegralType(ga, b.left(),addType);
					ga.visitIntInsn(ISTORE, 1);
					pushIntegralType(ga, b.right(),addType);
					ga.visitIntInsn(ISTORE, 2);
					ga.visitIntInsn(ILOAD, 1);
					ga.visitIntInsn(ILOAD, 2);
					//ga.visitInsn(op.get(addType));
	
					mw.visitIntInsn(ILOAD, 0);
	
					debugPrintLine(mw,addType);
					*/
				}

				if (instruction is AggregateNode)
				{
					JuliarLogger.log("BinaryNode");
					//Map<IntegralType,Integer> op = CodeGeneratorMap.generateMap(((AggregateNode)instruction).operation().toString());

					IList<IntegralTypeNode> integralTypeNodes = ((AggregateNode)instruction).data();
					int addCount = integralTypeNodes.Count - 1;

					if (isDebug)
					{
						mw.visitFieldInsn(GETSTATIC, "java/lang/System", "out", "Ljava/io/PrintStream;");
					}

					//Scan Type to typecast correctly
					IntegralType addType = IntegralType.jinteger;
					int[] anArray = new int[5];
					foreach (IntegralTypeNode integralTypeNode in integralTypeNodes)
					{
						switch (integralTypeNode.IntegralType)
						{
							case jdouble:
								anArray[0]++;
								break;
							case jfloat:
								anArray[1]++;
								break;
							case jlong:
								anArray[2]++;
								break;
							default:
								break;
						}
					}
					//
					if (anArray[0] != 0)
					{
						addType = IntegralType.jdouble;
					}
					else if (anArray[1] != 0)
					{
						addType = IntegralType.jfloat;
					}
					else if (anArray[2] != 0)
					{
						addType = IntegralType.jlong;
					}
					foreach (IntegralTypeNode integralTypeNode in integralTypeNodes)
					{
						pushIntegralType(ga, integralTypeNode,addType);
					}

					for (int i = 0; i < addCount; i++)
					{
						//ga.visitInsn(op.get(addType));
					}

					debugPrintLine(mw,addType);
				}
			}


			return mw;
		}

		private void debugPrintLine(MethodVisitor mw, IntegralType addType)
		{
			if (isDebug)
			{
				switch (addType)
				{
					case com.juliar.nodes.IntegralType.jdouble:
						mw.visitMethodInsn(INVOKEVIRTUAL, "java/io/PrintStream", "println", "(D)V", false);
						break;
					case com.juliar.nodes.IntegralType.jfloat:
						mw.visitMethodInsn(INVOKEVIRTUAL, "java/io/PrintStream", "println", "(F)V", false);
						break;
					case com.juliar.nodes.IntegralType.jinteger:
						mw.visitMethodInsn(INVOKEVIRTUAL, "java/io/PrintStream", "println", "(I)V", false);
						break;
					case com.juliar.nodes.IntegralType.jlong:
						mw.visitMethodInsn(INVOKEVIRTUAL, "java/io/PrintStream", "println", "(L)V", false);
						break;
					default:
						break;
				}
			}
		}

		private void pushIntegralType(GeneratorAdapter ga, Node node, IntegralType integralType)
		{
			if (node is IntegralTypeNode)
			{
				IntegralTypeNode integralTypeNode = ((IntegralTypeNode)node);
				switch (integralType)
				{
					case com.juliar.nodes.IntegralType.jdouble:
		 //               ga.push(Double.parseDouble(integralTypeNode.getIntegralValue()));
						break;
					case com.juliar.nodes.IntegralType.jfloat:
		 //               ga.push(Float.parseFloat(integralTypeNode.getIntegralValue()));
						break;
					case com.juliar.nodes.IntegralType.jinteger:
		//                ga.push(Integer.parseInt(integralTypeNode.getIntegralValue()));
						break;
					case com.juliar.nodes.IntegralType.jlong:
		//                ga.push(Long.parseLong(integralTypeNode.getIntegralValue()));
						break;
					default:
						break;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static byte[] getBytes(java.io.InputStream is) throws java.io.IOException
		private static sbyte[] getBytes(InputStream @is)
		{
			using (ByteArrayOutputStream os = new ByteArrayOutputStream(),)
			{
				sbyte[] buffer = new sbyte[0xFFFF];
				for (int len; (len = @is.read(buffer)) != -1;)
				{
					os.write(buffer, 0, len);
				}
				os.flush();
				return os.toByteArray();
			}
		}
	}

}