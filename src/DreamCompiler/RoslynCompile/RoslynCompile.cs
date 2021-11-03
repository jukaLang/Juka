using System;
using System.Collections.Generic;

namespace DreamCompiler.RoslynCompile
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    public class CompileRoslyn
    {
        public static string CompileSyntaxTree(SyntaxTree parsedSyntaxTree, String outputFile)
        {
            try
            {

                var rt = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                var references = new List<MetadataReference>() {
                    MetadataReference.CreateFromFile($@"{rt}System.Private.CoreLib.dll"),
                    MetadataReference.CreateFromFile($@"{rt}\System.dll"),
                    MetadataReference.CreateFromFile($@"{rt}\System.Console.dll"),
                    MetadataReference.CreateFromFile($@"{rt}\System.Runtime.dll"),
                };

                var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { parsedSyntaxTree}, references: references);

                //Emit to stream
                var ms = new MemoryStream();
                var emitResult = compilation.Emit(ms);

                var type = Assembly.Load(ms.GetBuffer()).GetType("MyClass");

                string allConsoleOutput = "";

                using (StringWriter stringWriter = new StringWriter())
                {
                    Console.SetOut(stringWriter);
                    type.InvokeMember("Main", BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);
                    allConsoleOutput = stringWriter.ToString();

                    var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                    standardOutput.AutoFlush = true;
                    Console.SetOut(standardOutput);
                }

                return allConsoleOutput;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
