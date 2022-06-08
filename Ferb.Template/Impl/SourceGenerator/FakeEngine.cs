namespace Ferb.Template.Impl.SourceGenerator;
internal class FakeEngine : IInstantiateTemplates
{
    public TemplateBase<TContext> Create<TContext>(string templateFile) { throw new NotImplementedException("Calls to sub templates are not implemented here. You're probably using the wrong class"); }
}