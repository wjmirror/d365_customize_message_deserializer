using SSCo.DigitalHighway.ServiceBus.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d365foMessage
{
    internal class MyServiceBusOptions : ServiceBusOptions
    {
        public string? ServiceBusConnectionKeyVault { get; set; }
    }
}
