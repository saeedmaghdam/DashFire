using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DashFire
{
    internal class QueueManager
    {
        private readonly IOptions<DashOptions> _options;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string _serviceSideExchangeName = "DashFire.Dashboard";
        private const string _serviceSideQueueName = "DashFire.Dashboard";

        private const string _dashboardSideExchangeName = "DashFire.Service";

        internal static QueueManager Instance = new QueueManager(JobContext.Instance.ServiceProvider);

        internal QueueManager(IServiceProvider serviceProvider)
        {
            _options = (IOptions<DashOptions>)serviceProvider.GetService(typeof(IOptions<DashOptions>));

            var factory = new ConnectionFactory() { Uri = new Uri(_options.Value.RabbitMqConnectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        internal void Initialize(string jobKey, string jobInstanceId)
        {
            _channel.ExchangeDeclare(_serviceSideExchangeName, "headers", true);
            _channel.QueueDeclare(queue: _serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(_serviceSideQueueName, _serviceSideExchangeName, string.Empty);

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

        internal void Publish(string message)
        {
            var messageBodyBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(_serviceSideExchangeName, "", null, messageBodyBytes);
        }
    }
}
