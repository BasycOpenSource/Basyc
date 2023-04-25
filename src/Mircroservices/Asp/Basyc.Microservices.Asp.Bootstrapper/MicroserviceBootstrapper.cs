using Basyc.Asp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basyc.MicroService.Asp.Bootstrapper;

public static class MicroserviceBootstrapper
{
    public static MicroserviceBuilder<IHostBuilder> CreateBuilder<TStartup>(string[] args)
        where TStartup : class, IStartupClass
    {
        var microserviceBuilder = Host.CreateDefaultBuilder()
            .CreateMicroserviceBuilder<TStartup>();

        return microserviceBuilder;
    }
}
