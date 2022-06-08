using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Runtime.Loader;
namespace Ferb.Template.Impl;

internal static class CompilationSyntaxTreeExtensions
{

    public static (EmitResult, Assembly?) CompileAssembly(this SyntaxTree tree, string assemblyName)
    {


        var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);

        var references = AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => !a.IsDynamic && !String.IsNullOrEmpty(a.Location))
          .Select(a => MetadataReference.CreateFromFile(a.Location));
        var code = tree.GetText().ToString();

        var compilation = CSharpCompilation.Create($"{assemblyName}.dll",
                  new[] { tree },
                  references: references,
                  options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                      optimizationLevel: OptimizationLevel.Debug,
                      assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        using (var peStream = new MemoryStream())
        {
            var result = compilation.Emit(peStream);
            if (result.Success)
            {
                return (result, AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(peStream.ToArray())));
            }
            else
            {
                return (result, null);
            }
        }

    }

}