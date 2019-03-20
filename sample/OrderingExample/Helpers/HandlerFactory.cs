namespace OrderingExample.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Core;
    using Extensions;
    using Persistence.Azure;
    using StructureMap;

    internal class HandlerFactory : IHandlerFactory
    {
        private readonly IContext ctx;

        public HandlerFactory(IContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task Dispatch(EventPublishedNotification notification)
        {
            var @event = notification.ToEvent();
            var subscriberType = Type.GetType(notification.SubscriberType);
            var subscriber = this.ctx.GetInstance(subscriberType);
            await subscriber.ConsumeSubscribedEvent(@event);
        }
    }
}
