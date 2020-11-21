using System;
using System.Collections.Generic;

namespace DreamCompiler.RoslynCompile
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;

    public class CompileRoslyn
    {
        private static readonly IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System",
                "System.IO",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Collections.Generic"
            };


        private static readonly string rt = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "{0}.dll";
        private static readonly IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(string.Format(rt, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(rt, "System")),
                MetadataReference.CreateFromFile(string.Format(rt, "System.Core"))
            };

        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        public static void CompileSyntaxTree(SyntaxTree parsedSyntaxTree, String outputFile)
        {
            try
            {
                var compilation = CSharpCompilation.Create(outputFile, syntaxTrees: new[] {parsedSyntaxTree}, references: DefaultReferences);
                var result = compilation.Emit(outputFile + ".exe", outputFile + ".pdb");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
                if (!result.Success)
                {
                    throw new Exception(result.Diagnostics[0].ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
