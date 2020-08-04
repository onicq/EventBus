using System.Threading.Tasks;

namespace EventBus
{
    public interface IIntegrationEventHandler<TE>
        where TE : IIntegrationEvent
    {
        Task HandleAsync(IIntegrationEvent @event);
    }
}
