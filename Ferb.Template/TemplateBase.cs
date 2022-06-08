using Ferb.Template.Impl;
using Ferb.Template.Impl.Precompilation;

namespace Ferb.Template;

public abstract class TemplateBase<TContext>
{
    protected IInstantiateTemplates _templateEngine;
    public TemplateBase(IInstantiateTemplates engine)
    {
        _templateEngine = engine;
        ClassInit();
    }
    private TContext? _context;
    private System.IO.StringWriter? _output;
    protected StringWriter Output => _output!;
    protected TContext Context => _context!;
    protected abstract void ExecInternal();
    protected virtual void BeforeExec() { }
    protected virtual void AfterExec() { }
    protected virtual void ClassInit() { }
    protected virtual string Transform(string outputText) => outputText;
    public string Exec(TContext context)
    {
        _output = new StringWriter();
        _context = context;
        BeforeExec();
        ExecInternal();
        AfterExec();
        return Transform(_output.ToString());
    }
    protected virtual string Call<TChildContext>(string template, TChildContext childContext)
    {
        TemplateBase<TChildContext> child = _templateEngine.Create<TChildContext>(template);
        return child.Exec(childContext);
    }
}
public abstract class PrecompiledTemplate<TContext> : TemplateBase<TContext>
{
    protected PrecompiledTemplate() : base(PrecompiledTemplateRegistry.Instance)
    {
    }

    protected override void ExecInternal()
    {
        throw new System.NotImplementedException();
    }
}