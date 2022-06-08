using System.Reflection;

namespace Ferb.Template.Impl;

public class EmbededResourceTemplateResolver : IResolveTemplates
{
    public static IResolveTemplates Create<T>(string viewFolder = null)
    {
        var parent = typeof(T).Namespace;
        if (viewFolder != null)
        {
            parent += "." + viewFolder;
        }
        return new EmbededResourceTemplateResolver(typeof(T).Assembly, parent);
    }
    private readonly Dictionary<string, string> _templates;


    public EmbededResourceTemplateResolver(Assembly assembly, string parentNamespace)
    {
        if (string.IsNullOrWhiteSpace(parentNamespace))
        {
            throw new ArgumentException($"'{nameof(parentNamespace)}' cannot be null or whitespace.", nameof(parentNamespace));
        }
        parentNamespace = parentNamespace.TrimEnd('.');

        _templates = assembly
          .GetManifestResourceNames()
          .Where(n => n.StartsWith(parentNamespace, StringComparison.InvariantCultureIgnoreCase))
          .ToDictionary(n => n.Replace(parentNamespace, "").TrimStart('.'), n => new StreamReader(assembly.GetManifestResourceStream(n)!).ReadToEnd(), StringComparer.InvariantCultureIgnoreCase);


    }
    public string Resolve(string name)
    {
        name = name.TrimStart(new[] { '/', '\\', '.' });
        name = name.Replace('\\', '.').Replace('/', '.');

        if (_templates.ContainsKey(name))
        {
            return _templates[name];
        }
        return String.Empty;

    }
}