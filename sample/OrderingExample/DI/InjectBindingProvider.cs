namespace OrderingExample.DI
{
    using System.Threading.Tasks;

    using Microsoft.Azure.WebJobs.Host.Bindings;

    using StructureMap;

    public class InjectBindingProvider : IBindingProvider
    {
        private readonly IContainer container;

        public InjectBindingProvider(IContainer container)
        {
            this.container = container;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding binding = new InjectBinding(this.container, context.Parameter.ParameterType);
            return Task.FromResult(binding);
        }
    }
}
