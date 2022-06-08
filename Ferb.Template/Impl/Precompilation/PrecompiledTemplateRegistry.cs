namespace Ferb.Template.Impl.Precompilation
{
    public class PrecompiledTemplateRegistry : IInstantiateTemplates
    {

        public static PrecompiledTemplateRegistry Instance = new PrecompiledTemplateRegistry();
        private Dictionary<string, Type> _registry = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        public virtual void Register<TTemplate, TContext>(string templateFile) where TTemplate : TemplateBase<TContext>
        {
            _registry[CreateKey(typeof(TContext), templateFile)] = typeof(TTemplate);
        }

        private static string CreateKey(Type contextType, string templateFile)
        {
            return $"{templateFile}:{contextType.FullName}";
        }

        public virtual TemplateBase<TContext> Create<TContext>(string templateFile)
        {
            var key = CreateKey(typeof(TContext), templateFile);
            if (!_registry.ContainsKey(key))
            {
                throw new NotImplementedException($"No implementation for {key} was registered");
            }
            return (TemplateBase<TContext>)Activator.CreateInstance(_registry[key]);
        }
    }
}