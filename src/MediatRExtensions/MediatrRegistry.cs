namespace MediatRExtensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MediatR;
    using StructureMap;
    using StructureMap.Pipeline;
    using StructureMap.TypeRules;

    public class MediatrRegistry : Registry
    {
        public MediatrRegistry(params Type[] assembliesContainingTypes)
            : this(assembliesContainingTypes.Select(t => t.GetAssembly()).ToArray())
        {
        }

        public MediatrRegistry(params Assembly[] requestAssemblies)
        {
            this.Scan(scanner =>
            {
                foreach (var assembly in requestAssemblies)
                {
                    scanner.Assembly(assembly);
                }

                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(IRequestHandler<>));
                scanner.AddAllTypesOf(typeof(IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });
            this.For<IMediator>().LifecycleIs<TransientLifecycle>().Use<Mediator>();
            this.For<ServiceFactory>().Use("HandlerFactory", HandlerFactory());
        }

        private static Func<IContext, ServiceFactory> HandlerFactory()
        {
            return ctx => t =>
            {
                var instance = ctx.TryGetInstance(t);
                return instance;
            };
        }
    }
}
