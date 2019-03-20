namespace OrderingExample.DI
{
    using Microsoft.Azure.WebJobs.Host.Config;
    using StructureMap;

    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var container = new Container(new FunctionsRegistry());
            context
                .AddBindingRule<InjectAttribute>()
                .Bind(new InjectBindingProvider(container));
        }
    }
}
