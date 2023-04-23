using Basyc.Asp;
using Basyc.MessageBus.Client;
using Basyc.MicroService.Dapr.MessageBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Basyc.MicroService.Asp.Dapr;

public static class WebHostBuilderDaprExtensions
{
    public static IWebHostBuilder ConfigureDaprServices(this IWebHostBuilder webBuilder)
    {
        webBuilder.ConfigureServices((context, services) =>
        {
            services.AddTransient<IStartupFilter, DaprStartupFilter>();
            services.AddDaprClient();
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            services.AddControllers().FixJsonSerialization().AddDapr();

            services.Configure<DaprMessageBusManagerOptions>(options =>
            {
                options.PubSubName = MessageBusConstants.MessageBusName;
            });
        });

        //var serviceBuilder = new MicroserviceBuilder(webBuilder,IHostBuilder).AddDaprProvider();
        //throw new Exception();
        //configure(serviceBuilder);

        return webBuilder;
    }
}
