using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace d365framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var vaultName = System.Configuration.ConfigurationManager.AppSettings["ServiceBusConnectionKeyVault"];
            var kvUri = $"https://{vaultName}.vault.azure.net";
            var kvClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            var secret = Task.Run(async () => await kvClient.GetSecretAsync("ServiceBusConnection")).Result;
            var serviceBusConnection = secret.Value.Value;

            TopicClient topicClient = TopicClient.CreateFromConnectionString(serviceBusConnection, "dynamics-fo.purchasinginvoice");

            var message = new PurchaseOrderMessage()
            {
                PurchaseOrderLevelInformation = new PurchaseOrderLevelInformation()
                {
                    InvoiceDate = "2024-02-07",
                    InvoiceId = "RY36201",
                    LedgerVoucher = "IVR0000010"
                },
                LineItems = new List<LineItem>()
                {
                    new LineItem()
                    {
                        PurchId="1000",
                        ProcurementProductCategoryName="Item 1",
                        ProcurementProductCategoryHierarchyName="WebERP",
                        PurchaseLineLineNumber=1,
                        PurchPrice= 7.61m,
                        PriceUnit= 1.0m
                    },
                    new LineItem()
                    {
                        PurchId="1000",
                        ProcurementProductCategoryName="Item 2",
                        ProcurementProductCategoryHierarchyName="WebERP",
                        PurchaseLineLineNumber=1,
                        PurchPrice= 7.61m,
                        PriceUnit= 2.0m
                    }
                }
            };
            string json = JsonSerializer.Serialize(message);

            BrokeredMessage brokeredMessage = new BrokeredMessage(json);

            //System.IO.MemoryStream stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            //BrokeredMessage brokeredMessage = new BrokeredMessage(stream, true);

            topicClient.Send(brokeredMessage);


        }
    }
}
