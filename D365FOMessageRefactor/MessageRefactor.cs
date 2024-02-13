using System;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SSCo.DigitalHighway.ServiceBus.Contracts;

namespace D365FOMessageRefactor;

public class MessageRefactor
{
    private readonly ILogger<MessageRefactor> _logger;

    public MessageRefactor(ILogger<MessageRefactor> logger)
    {
        _logger = logger;
    }

    [Function(nameof(MessageRefactor))]
    [ServiceBusOutput("d365fo/purchase.invoiceevent", entityType: ServiceBusEntityType.Topic, Connection = "ServiceBusConnection")]
    public Message<PurchaseOrderMessage> Run([ServiceBusTrigger("dynamics-fo.purchasinginvoice", "azurerefactor", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
    {
        var serializer = new DataContractBinarySerializer(typeof(string));
        var stringMessage = (string)serializer.ReadObject(message.Body.ToStream());
        _logger.LogInformation($"MessageRefactor receive message:\r\n{stringMessage}");

        //forward 
        var purchaseOrder = System.Text.Json.JsonSerializer.Deserialize<PurchaseOrderMessage>(stringMessage);

        var purchaseMessage = new Message<PurchaseOrderMessage>()
        {
            Name = "Invoice.Received",
            Content = purchaseOrder
        };

        return purchaseMessage;
    }
}
