using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using EmpTracker.EmpService.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmpTracker.EmpService.Infrastructure.Messaging
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private string _consumerTag = string.Empty;

        public MessagePublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration.GetSection("RabbitMq").GetSection("Uri").Value!),
                ClientProvidedName = "Emp Publisher"
            };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task PublishAsync<T>(T message, string queue, string exchange, string routingKey) where T : class
        {
            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
            await _channel.QueueDeclareAsync(queue, false, false, false, null);
            await _channel.QueueBindAsync(queue, exchange, routingKey, null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var props = new BasicProperties
            {
                ContentType = "text/plain",
                DeliveryMode = DeliveryModes.Persistent
            };
            await _channel.BasicPublishAsync(exchange, routingKey, true, props, body);
        }

        public async Task SubscribeAsync<T>(string queue, string exchange, string routingKey, Func<T, Task> handleMessage) where T : class
        {
            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
            await _channel.QueueDeclareAsync(queue, false, false, false, null);
            await _channel.QueueBindAsync(queue, exchange, routingKey, null);
            await _channel.BasicQosAsync(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(body);
                if (message != null)
                {
                    await handleMessage(message);
                }

                await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            };

            _consumerTag = await _channel.BasicConsumeAsync(queue, autoAck: false, consumer);
        }

        public void Dispose()
        {
            _channel.BasicCancelAsync(_consumerTag).GetAwaiter().GetResult();
            _channel.CloseAsync().GetAwaiter().GetResult();
            _connection.CloseAsync().GetAwaiter().GetResult();

            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
