using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpTracker.EmpService.Core.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queue, string exchange, string routingKey) where T : class;
        Task SubscribeAsync<T>(string queue, string exchange, string routingKey, Func<T, Task> handleMessage) where T : class;
    }
}
