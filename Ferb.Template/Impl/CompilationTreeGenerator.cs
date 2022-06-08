using Ferb.Template.Impl.SourceGenerator;

using Microsoft.CodeAnalysis;

namespace Ferb.Template.Impl;

public record CompilationTreeGenerator(IResolveTemplates Resolver, IRecognizeBoundries Boundries)
{
    public (SyntaxTree tree, string className) GenerateDynamic<TContext>(string templateFile)
    {
        var ctx = GenerationConext.Parse<TContext>(templateFile, Resolver, Boundries, isPrecompiled: false);
        var tree = CSharpImplGenerator.GenerateFor(ctx);
        return (tree, ctx.ClassName);

    }
    public SyntaxTree GeneratePrecompiled<TContext>(string templateFile, string @namespace, string @class) =>
        CSharpImplGenerator.GenerateFor(GenerationConext.Parse<TContext>(templateFile, Resolver, Boundries, @namespace, @class, true));

}