using System;
using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MediatR;
using EmpTracker.EmpService.Application.Features.Employees.Queries;
using EmpTracker.EmpService.Core.Interfaces;

namespace EmpTracker.EmpService.Infrastructure.Messaging
{
    public class EmployeeSubscriberService : IHostedService
    {

        private readonly IEventProcessor _eventProcessor;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private string _consumerTag = string.Empty;

        public EmployeeSubscriberService(IEventProcessor eventProcessor, IConfiguration configuration)
        {
            _eventProcessor = eventProcessor;
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration.GetSection("RabbitMq").GetSection("Uri").Value!),
                ClientProvidedName = "Subscriber"
            };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //await _channel.ExchangeDeclareAsync("empTracker.direct", ExchangeType.Direct, cancellationToken: cancellationToken);
            //await _channel.QueueDeclareAsync("employee_queue", false, false, false, null, cancellationToken: cancellationToken);
            //await _channel.BasicQosAsync(0, 1, false, cancellationToken: cancellationToken);

            //var consumer = new AsyncEventingBasicConsumer(_channel);
            //consumer.ReceivedAsync += async (sender, args) =>
            //{
            //    await _eventProcessor.ProcessEvent(args);
            //    await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            //};

            //_consumerTag = await _channel.BasicConsumeAsync("employee_queue", autoAck: false, consumer, cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _channel.BasicCancelAsync(_consumerTag, cancellationToken: cancellationToken);
            await _channel.CloseAsync(cancellationToken);
            await _connection.CloseAsync(cancellationToken);
        }
    }
}
