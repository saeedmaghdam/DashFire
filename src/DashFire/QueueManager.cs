using System;
using System.Collections.Generic;
using System.Text;
using DashFire.Constants;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DashFire
{
    /// <summary>
    /// Queue manager
    /// </summary>
    public class QueueManager
    {
        private readonly IOptions<DashOptions> _options;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string _serviceSideExchangeName = "DashFire.Dashboard";
        private const string _serviceSideQueueName = "DashFire.Dashboard";

        private const string _dashboardSideExchangeName = "DashFire.Service";

        /// <summary>
        /// Queue manager constructor.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        public QueueManager(IServiceProvider serviceProvider)
        {
            _options = (IOptions<DashOptions>)serviceProvider.GetService(typeof(IOptions<DashOptions>));

            var factory = new ConnectionFactory() { Uri = new Uri(_options.Value.RabbitMqConnectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        internal void Initialize(string jobKey, string jobInstanceId)
        {
            // Declare dashboard exchanges and queue
            _channel.ExchangeDeclare(_serviceSideExchangeName, "headers", true);

            var serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.Registration}";
            _channel.QueueDeclare(queue: serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(serviceSideQueueName, _serviceSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "message_type", MessageTypes.Registration.ToString().ToLower()
                }
            });

            // Declare dashboard exchanges and queue
            var dashboardSideQueueName = $"{_dashboardSideExchangeName}_{jobKey}_{jobInstanceId}";
            _channel.ExchangeDeclare(_dashboardSideExchangeName, "headers", true);
            _channel.QueueDeclare(queue: dashboardSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(dashboardSideQueueName, _dashboardSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "job_key", jobKey
                },
                {
                    "job_instance_id", jobInstanceId
                }
            });
        }

        internal void Publish(MessageTypes messageType, string message)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = false;
            properties.Headers = new Dictionary<string, object>()
            {
                {
                    "message_type", messageType.ToString().ToLower()
                }
            };
            var messageBodyBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(_serviceSideExchangeName, "", properties, messageBodyBytes);
        }
    }
}
