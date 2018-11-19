using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.RoslynCompile
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using System.Runtime.CompilerServices;

    public class CompileRoslyn
    {
        private static System.Collections.Generic.IEnumerable<string> DefaultNamespaces =
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
        
        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\{0}.dll";
        private static readonly System.Collections.Generic.IEnumerable<MetadataReference> DefaultReferences =
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

        public static void CompileSyntaxTree(SyntaxTree parsedSyntaxTree)
        {
            //var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            //var systemLib = MetadataReference.CreateFromFile(string.Format(rt, "System"));
            ///var systemCoreLib = MetadataReference.CreateFromFile(string.Format(rt, "System.Core"));

            //var compilation = CSharpCompilation.Create("Test", syntaxTrees: new[] { parsedSyntaxTree }, references: new[] { mscorlib , systemCoreLib, systemLib});
            var compilation = CSharpCompilation.Create("Test", syntaxTrees: new[] {parsedSyntaxTree}, references: DefaultReferences);

            try
            {
                var result = compilation.Emit(@"Test.exe", @"test.pdb");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
                if (!result.Success)
                {
                    throw new Exception("error");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
