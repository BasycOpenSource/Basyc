using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Asp;

public interface IStartupClass
{
    void ConfigureServices(IServiceCollection services);

    void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}
