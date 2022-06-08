namespace Ferb.Template.Precompiled;

[AttributeUsage(AttributeTargets.Class)]
public class TemplateAttribute : Attribute
{
    public TemplateAttribute(string fileName, Type contextType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
        }

        FileName = fileName;
        ContextType = contextType;
    }
    public string FileName { get; private set; }
    public Type ContextType { get; }
}