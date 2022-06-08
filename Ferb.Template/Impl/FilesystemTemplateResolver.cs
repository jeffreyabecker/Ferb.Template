namespace Ferb.Template.Impl;

public class FilesystemTemplateResolver : IResolveTemplates
{
    private readonly string _baseDir;

    public FilesystemTemplateResolver(string baseDir)
    {
        if (string.IsNullOrWhiteSpace(baseDir))
        {
            throw new ArgumentException($"'{nameof(baseDir)}' cannot be null or whitespace.", nameof(baseDir));
        }

        _baseDir = baseDir;
    }
    public string Resolve(string name)
    {
        return File.ReadAllText(Path.Combine(_baseDir, name.TrimStart(new[] { '/', '\\', '~' })));
    }
}