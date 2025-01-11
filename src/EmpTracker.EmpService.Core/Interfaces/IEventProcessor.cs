using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace EmpTracker.EmpService.Core.Interfaces
{
    public interface IEventProcessor
    {
        Task ProcessEvent(BasicDeliverEventArgs args);
    }
}
