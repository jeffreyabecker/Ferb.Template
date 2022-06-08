using Ferb.Template.Impl;
using Microsoft.CodeAnalysis;

namespace Ferb.Template;

public class Engine : IInstantiateTemplates
{

    private readonly Dictionary<string, Type> _cache = new Dictionary<string, Type>();
    private static Engine? _instance;
    public static IInstantiateTemplates TemplateEngine(string dir = "templates", [System.Runtime.CompilerServices.CallerFilePath] string? path = null)
    {
        if (_instance == null)
        {
            _instance = new Engine(Path.Combine(Path.GetDirectoryName(path)!, dir));
        }
        return _instance;
    }
    private readonly CompilationTreeGenerator _gen;
    public Engine(CompilationTreeGenerator gen)
    {
        _gen = gen;
    }
    public Engine(string dir, BoundryStyle? style = null)
    {
        _gen = new CompilationTreeGenerator(new FilesystemTemplateResolver(dir), new BoundryRecognizer(style ?? BoundryStyle.Asp));
    }

    public string Exec<TContext>(string templateFile, TContext context)
    {
        var template = Create<TContext>(templateFile);
        return template.Exec(context);

    }
    public TemplateBase<TContext> Create<TContext>(string templateFile)
    {
        var key = $"{templateFile}:{typeof(TContext).AssemblyQualifiedName}";
        if (!_cache.ContainsKey(key))
        {
            _cache[key] = CreateTemplateImpl<TContext>(templateFile);
        }
        return (TemplateBase<TContext>)Activator.CreateInstance(_cache[key], new object[] { this })!;
    }



    private Type CreateTemplateImpl<TContext>(string templateFile)
    {

        var (tree, className) = _gen.GenerateDynamic<TContext>(templateFile);
        var references = AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => !a.IsDynamic && !String.IsNullOrEmpty(a.Location))
          .Select(a => MetadataReference.CreateFromFile(a.Location));

        var (result, assembly) = tree.CompileAssembly(className);

        TemplateCompilationException.ThrowIfFailed(result);

        return assembly!.GetTypes().First(c => c.Name == className);
    }




}