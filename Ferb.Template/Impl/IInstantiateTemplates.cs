namespace Ferb.Template.Impl;

public interface IInstantiateTemplates
{
    TemplateBase<TContext> Create<TContext>(string templateFile);
}