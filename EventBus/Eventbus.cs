using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EventBus
{
    public class EventBus : IEventBus
    {
        private readonly IConnection _connection;
        private IModel _channel;

        public IModel Channel 
        {
            get 
            {
                if (_channel == null)
                    _channel = _connection.CreateModel();
                
                return _channel;
            }
        }
        public EventBus(IConfiguration config)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = config["EventBus:HostName"],
                Port = int.Parse(config["EventBus:Port"]),
                UserName = config["EventBus:UserName"],
                Password = config["EventBus:Password"],
                DispatchConsumersAsync = true
            };


            _connection = connectionFactory.CreateConnection();
        }
        public void Publish(IIntegrationEvent @event, string exchangeName)
        {
            CreateExchangeIfNotExists(exchangeName);

            var jsonEvent = JsonConvert.SerializeObject(@event);
            var bytesEvent = Encoding.UTF8.GetBytes(jsonEvent);

            Channel.BasicPublish(exchangeName, routingKey: string.Empty, body: bytesEvent);
        }

        public void Subscribe<TH, TE>(string exchangeName, string subscribeName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent
        {
            BindQueue(exchangeName, subscribeName);
        }

        private void CreateExchangeIfNotExists(string exchangeName) 
        {
            Channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true);
        }

        private void BindQueue(string exchangeName, string subscribeName) 
        {
            CreateExchangeIfNotExists(exchangeName);

            Channel.QueueDeclare(queue: subscribeName, durable: true, exclusive: false, autoDelete: false);
            Channel.QueueBind(subscribeName, exchangeName, routingKey: string.Empty);
        }
    }
}
