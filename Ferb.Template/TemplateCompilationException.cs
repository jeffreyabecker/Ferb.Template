using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Text;

namespace Ferb.Template;

public class TemplateCompilationException : Exception
{
    public static void ThrowIfFailed(EmitResult result)
    {
        if (!result.Success)
        {
            throw new TemplateCompilationException(result);
        }
    }
    public TemplateCompilationException(EmitResult result) : base(ConstructMessage(result))
    {

    }

    private static string ConstructMessage(EmitResult result) => result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error)
        .Aggregate(new StringBuilder(), (sb, d) => sb.AppendLine($"{d.Id,-10}: {d.GetMessage()}"))
        .ToString();
}