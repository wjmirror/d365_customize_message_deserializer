using d365framework;
using MassTransit;
using Microsoft.Extensions.Logging;
using SSCo.DigitalHighway.ServiceBus.Infrastructure;

namespace d365foMessage
{
    public class PurchaseOrderMessageConsumer : IConsumer<PurchaseOrderMessage>
    {
        private readonly ILogger<MessageConsumerBase<PurchaseOrderMessage>> _logger;

        public PurchaseOrderMessageConsumer(ILogger<MessageConsumerBase<PurchaseOrderMessage>> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PurchaseOrderMessage> context)
        {
            var str = System.Text.Json.JsonSerializer.Serialize(context.Message);
            Console.WriteLine(str);
            return Task.CompletedTask;
        }
    }
}