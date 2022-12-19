using Basyc.Asp;
using Basyc.MicroService.Abstraction.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MicroService.Asp.Bootstrapper
{
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
}