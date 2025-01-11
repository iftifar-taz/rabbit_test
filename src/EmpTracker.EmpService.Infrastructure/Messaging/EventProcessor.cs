using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EmpTracker.EmpService.Application.Features.Employees.Queries;
using EmpTracker.EmpService.Core.Interfaces;
using EmpTracker.EmpService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;

namespace EmpTracker.EmpService.Infrastructure.Messaging
{
    public class EventProcessor(IServiceScopeFactory scopeFactory) : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task ProcessEvent(BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = JsonSerializer.Deserialize<EmployeeMessage>(body);


            if (args.RoutingKey == "employee.created")
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    //var a = await mediator.Send(new GetEmployeesQuery());
                    var b = args.RoutingKey;
                }
                catch (Exception ex)
                {
                    var b = args.RoutingKey;
                }
            }

        }

        public record EmployeeMessage(string Name, string Department);
    }
}
