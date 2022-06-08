using System.Text.RegularExpressions;

namespace Ferb.Template.Impl;

internal record GenerationConext(string FileName, string Namespace, string ClassName, string ContextClassName, System.Collections.Generic.List<Token> Tokens, bool IsPrecompiled)
{
    internal static GenerationConext Parse<TContext>(string fileName, IResolveTemplates resolver, IRecognizeBoundries boundries, string? @namespace = null, string? className = null, bool isPrecompiled = false) =>
        Parse(fileName, resolver, boundries, typeof(TContext).FullName, @namespace, className, isPrecompiled);
    internal static GenerationConext Parse(string fileName, IResolveTemplates resolver, IRecognizeBoundries boundries, string contextType, string? @namespace = null, string? className = null, bool isPrecompiled = false)
    {
        @namespace = @namespace ?? "ns_" + Regex.Replace(fileName, "[^a-zA-Z0-9_]", "_");
        className = className ?? "cls_" + Regex.Replace(fileName, "[^a-zA-Z0-9_]", "_") + "_" + Regex.Replace(contextType, "[^a-zA-Z0-9_]", "_");
        var sourceText = resolver.Resolve(fileName);
        var tokens = Token.Parse(sourceText, boundries).ToList();
        return new GenerationConext(fileName, @namespace, className, contextType, tokens, isPrecompiled);
    }

}