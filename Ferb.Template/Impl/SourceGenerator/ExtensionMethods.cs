using Microsoft.CodeAnalysis;

namespace Ferb.Template.Impl.SourceGenerator;
internal static class ExtensionMethods
{
    public static string FullNamespace(this ISymbol symbol)
    {
        var parts = new Stack<string>();
        INamespaceSymbol? iterator = (symbol as INamespaceSymbol) ?? symbol.ContainingNamespace;
        while (iterator != null)
        {
            if (!string.IsNullOrEmpty(iterator.Name))
                parts.Push(iterator.Name);
            iterator = iterator.ContainingNamespace;
        }
        return string.Join(".", parts);
    }

    public static string? FullName(this INamedTypeSymbol? symbol)
    {
        if (symbol == null)
            return null;

        var prefix = FullNamespace(symbol);
        var suffix = "";
        if (symbol.Arity > 0)
        {
            suffix = "<" + string.Join(", ", symbol.TypeArguments.Select(targ => FullName((INamedTypeSymbol)targ))) + ">";
        }

        if (prefix != "")
            return prefix + "." + symbol.Name + suffix;
        else
            return symbol.Name + suffix;
    }

}