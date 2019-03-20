namespace OrderingExample.DI
{
    using Config;
    using Microsoft.Extensions.Options;
    using StructureMap;

    public static class RegistryOptionsExtensions
    {
        public static void UseOptions<T>(this IProfileRegistry registry, string configSection)
            where T : class, new()
        {
            registry.For<IOptions<T>>().Use(() => new OptionsProvider<T>(configSection)).Singleton();
        }
    }
}
