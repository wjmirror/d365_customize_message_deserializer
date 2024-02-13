
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using d365framework;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SSCo.DigitalHighway.ServiceBus.Infrastructure;

namespace d365foMessage;
class Program
{
    private static ILogger<Program> _logger = default!;

    static async Task Main(string[] args)
    {
        var _host = Host.CreateDefaultBuilder()
            .UseConsoleLifetime()
            .ConfigureLogging((context, logbuilder) =>
            {
                logbuilder.ClearProviders();
                logbuilder.AddDebug();
                logbuilder.AddConsole();
            })
            .ConfigureServices((context, services) => ConfigureServices(context, services))
            .Build();

        _logger = _host.Services.GetRequiredService<ILogger<Program>>();
        _logger.LogInformation($"TestStartup Host Initialized.");

        await _host.StartAsync();


        await _host.WaitForShutdownAsync();
    }

    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var configuration = context.Configuration;
        var busOptions = configuration.GetSection(nameof(ServiceBusOptions)).Get<MyServiceBusOptions>();
        if (string.IsNullOrWhiteSpace(busOptions.ServiceBusConnection) && !string.IsNullOrWhiteSpace(busOptions.ServiceBusConnectionKeyVault))
        {
            //get the connection string from keyvault
            var kvUri = "https://" + busOptions.ServiceBusConnectionKeyVault + ".vault.azure.net";
            var kvClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            var secret = Task.Run(async () => await kvClient.GetSecretAsync("ServiceBusConnection")).Result;
            busOptions.ServiceBusConnection = secret.Value.Value;
        }

        services.AddMassTransitAzureServiceBus(
            serviceBusOption => serviceBusOption.ServiceBusConnection = busOptions.ServiceBusConnection,
            busBuilder =>
            {
                busBuilder.AddSubscriptionConsumer<PurchaseOrderMessage, PurchaseOrderMessageConsumer>("d365fo-test", "test",
                    endcfg =>
                    {
                        endcfg.ClearSerialization();
                        var factory = new DataContractBinarySerializerFactory();
                        endcfg.AddSerializer(factory, true);
                        endcfg.AddDeserializer(factory, true);
                    });
            });

    }
}

