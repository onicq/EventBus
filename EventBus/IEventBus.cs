using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IEventBus
    {
        void Subscribe<TH, TE>(string exchangeName, string subscribeName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent;
        void Publish(IIntegrationEvent @event, string exchangeName);
    }
}
