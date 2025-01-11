using Asp.Versioning;
using EmpTracker.EmpService.Application.Dtos;
using EmpTracker.EmpService.Application.Features.Employees.Commands;
using EmpTracker.EmpService.Application.Features.Employees.Queries;
using EmpTracker.EmpService.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmpTracker.EmpService.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/messages")]
    public class MessageController(IMessageBus messageBus) : ControllerBase
    {
        private readonly IMessageBus _messageBus = messageBus;

        [HttpPost("publish")]
        public async Task<IActionResult> PublishMessage([FromBody] EmployeeMessage message)
        {
            await _messageBus.PublishAsync(message, "employee_queue", "empTracker.direct", "employee.created");
            return Ok("Message published.");
        }

        [HttpGet("subscribe")]
        public IActionResult SubscribeMessages()
        {
            _messageBus.SubscribeAsync<EmployeeMessage>("employee_queue", "empTracker.direct", "employee.created", HandleEmployeeMessageAsync);

            return Ok("Subscribed to messages.");
        }

        private async Task HandleEmployeeMessageAsync(EmployeeMessage message)
        {
            Console.WriteLine($"Received Message: {message.Name}");
            await Task.CompletedTask;
        }

        public record EmployeeMessage(string Name, string Department);
    }
}
