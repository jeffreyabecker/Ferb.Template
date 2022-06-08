using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Ferb.Template.Impl.SourceGenerator;
//https://github.com/Grauenwolf/Tortuga-TestMonkey/blob/main/Tortuga.TestMonkey/TestGenerator.cs
[Generator]
public class TemplateSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {


        if (!(context.SyntaxContextReceiver is TemplateGenerationSyntaxReciever receiver))
            return;
        context.AddSource("TemplateSourceGeneratorLog", SourceText.From($@"/*{ Environment.NewLine + string.Join(Environment.NewLine, receiver.Log) + Environment.NewLine}*/", Encoding.UTF8));

        context.Compilation.GetDiagnostics().AddRange(context.AdditionalFiles.Select(f => Diagnostic.Create(new DiagnosticDescriptor(f.Path, "debug", "path", f.Path, DiagnosticSeverity.Error, true), null)));
        foreach (var f in context.AdditionalFiles)
        {
            context.AddSource("TemplateSourceGeneratorLog", SourceText.From($@"/* Found additional file:{f.Path}*/", Encoding.UTF8));
        }
        //foreach (var todo in receiver.Templates)
        //{

        //}
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new TemplateGenerationSyntaxReciever());
    }

    internal record TemplateGenerationTodoItem(string FileName, string SourceFile, string Namespace, string ImplementationType, string ContextType);
    internal class TemplateGenerationSyntaxReciever : ISyntaxContextReceiver
    {
        public List<string> Log { get; } = new();
        public List<TemplateGenerationTodoItem> Templates { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {

            try
            {
                if (context.Node is CompilationUnitSyntax cmpu)
                {
                    Templates.AddRange(ExtractPrecompiledTemplateDeclarations(cmpu));
                }

            }
            catch (Exception ex)
            {
                Log.Add("Error parsing syntax: " + ex.ToString());
            }

        }


        public static IEnumerable<TemplateGenerationTodoItem>
          ExtractPrecompiledTemplateDeclarations(CompilationUnitSyntax context)
          => context.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Select(x => ExtractPrecompiledInfo(x))
            .Where(x => x.@namespace != null)
            .Select(x => new TemplateGenerationTodoItem(context.SyntaxTree.FilePath, x.path!, x.@namespace!, x.@namespace!, x.contextType!));
        private static (string? @namespace, string? className, string? contextType, string? path) ExtractPrecompiledInfo(ClassDeclarationSyntax declaration)
        {
            string? templateTypeName = null;
            string? contextTypeName = null;
            string? path = null;
            string? @namespace = declaration.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name?.ToFullString();


            templateTypeName = declaration.Identifier.Text;
            var baseType = declaration?.BaseList?.Types
              .OfType<SimpleBaseTypeSyntax>()
              .Where(b =>
              {
                  if (b.Type is GenericNameSyntax ss)
                  {
                      return ss.Identifier.Text == "PrecompiledTemplate";
                  }
                  return false;
              })
              .FirstOrDefault()
              .Type as GenericNameSyntax;

            if (baseType?.TypeArgumentList.Arguments[0] is IdentifierNameSyntax ctxName)
            {
                contextTypeName = ctxName.Identifier.Text;
            }


            var members = declaration?.ChildNodes().OfType<FieldDeclarationSyntax>()
              .Where(d => d.Declaration.Variables.Any(v => IsSourceFileVariable(v)))
              .FirstOrDefault()
              ?.Declaration;
            if (declaration != null)
            {
                var field = members?.Variables.First(v => IsSourceFileVariable(v));

                if (field?.Initializer?.Value is LiteralExpressionSyntax literal)
                {
                    path = literal.Token.ValueText;
                }
            }
            if (@namespace != null && templateTypeName != null && contextTypeName != null && path != null)
            {
                return (@namespace, templateTypeName, contextTypeName, path);
            }
            return (null, null, null, null);
        }
        private static bool IsSourceFileVariable(VariableDeclaratorSyntax v)
        {
            bool isRightName = v.Identifier.Text.TrimStart('_').Equals("SourceFile", StringComparison.OrdinalIgnoreCase);

            return isRightName;
        }
    }
}