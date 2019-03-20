namespace OrderingExample.DI
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Microsoft.Azure.WebJobs.Host.Protocols;

    using StructureMap;

    public class InjectBinding : IBinding
    {
        private readonly Type type;
        private readonly IContainer container;

        public InjectBinding(IContainer container, Type type)
        {
            this.type = type;
            this.container = container;
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult((IValueProvider)new InjectValueProvider(value));
        }

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            var value = this.container.GetInstance(this.type);
            return await this.BindAsync(value, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();

        private class InjectValueProvider : IValueProvider
        {
            private readonly object value;

            public InjectValueProvider(object value) => this.value = value;

            public Type Type => this.value.GetType();

            public Task<object> GetValueAsync() => Task.FromResult(this.value);

            public string ToInvokeString() => this.value.ToString();
        }
    }
}
