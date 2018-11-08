using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.RoslynCompile
{

    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class CompileRoslyn
    {
        private static readonly System.Collections.Generic.IEnumerable<string> DefaultNamespaces =
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

        private static readonly System.Collections.Generic.IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(string.Format(rt, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(rt, "System")),
                MetadataReference.CreateFromFile(string.Format(rt, "System.Core"))
            };

        public static void CompileSyntaxTree(SyntaxTree parsedSyntaxTree)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            //var compilation = CSharpCompilation.Create("Test", syntaxTrees: new[] { parsedSyntaxTree }, references: new[] { mscorlib });
            var compilation = CSharpCompilation.Create("Test", syntaxTrees: new[] {parsedSyntaxTree}, references: DefaultReferences);

            try
            {
                var result = compilation.Emit(@"Test.exe", @"test.pdb");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
