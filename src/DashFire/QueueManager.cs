using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DashFire.Constants;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        /// Queue consumer message handler
        /// </summary>
        /// <param name="jobKey">Job's key.</param>
        /// <param name="jobInstanceId">Job's instance id.</param>
        /// <param name="messageType">message's type.</param>
        /// <param name="message">message.</param>
        /// <returns>Returns a task.</returns>
        public delegate Task ConsumeHandler(string jobKey, string jobInstanceId, string messageType, string message);

        /// <summary>
        /// Event raise when a new message received.
        /// </summary>
        public event ConsumeHandler Received;

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

            var serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.HeartBit}";
            _channel.QueueDeclare(queue: serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(serviceSideQueueName, _serviceSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "message_type", MessageTypes.HeartBit.ToString().ToLower()
                }
            });

            serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.Registration}";
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

            serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.JobStatus}";
            _channel.QueueDeclare(queue: serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(serviceSideQueueName, _serviceSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "message_type", MessageTypes.JobStatus.ToString().ToLower()
                }
            });

            serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.LogJobStatus}";
            _channel.QueueDeclare(queue: serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(serviceSideQueueName, _serviceSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "message_type", MessageTypes.LogJobStatus.ToString().ToLower()
                }
            });

            serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.JobSchedule}";
            _channel.QueueDeclare(queue: serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(serviceSideQueueName, _serviceSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "message_type", MessageTypes.JobSchedule.ToString().ToLower()
                }
            });

            serviceSideQueueName = $"{_serviceSideQueueName}_{MessageTypes.Shutdown}";
            _channel.QueueDeclare(queue: serviceSideQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(serviceSideQueueName, _serviceSideExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "message_type", MessageTypes.Shutdown.ToString().ToLower()
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

        internal void StartConsume(string jobKey, string jobInstanceId)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ConsumerReceived;

            _channel.BasicConsume($"{_dashboardSideExchangeName}_{jobKey}_{jobInstanceId}", true, consumer);
        }

        private void ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var headers = e.BasicProperties;
            headers.Headers.TryGetValue("job_key", out var jobKeyBytes);
            headers.Headers.TryGetValue("job_instance_id", out var jobInstanceIdBytes);
            headers.Headers.TryGetValue("message_type", out var messageTypeBytes);

            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var jobKey = Encoding.UTF8.GetString((byte[])jobKeyBytes);
            var jobInstanceId = Encoding.UTF8.GetString((byte[])jobInstanceIdBytes);
            var messageType = Encoding.UTF8.GetString((byte[])messageTypeBytes);

            Received?.Invoke(jobKey, jobInstanceId, messageType, message);
        }
    }
}
