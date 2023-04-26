using Basyc.MessageBus.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basyc.MicroService.Asp.Dapr;

public class DaprStartupFilter : IStartupFilter
{
    public DaprStartupFilter(ITypedMessageBusClient messageBusManager)
    {
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => (app) =>
                                                                                           {
                                                                                               Configure(app);
                                                                                               next(app);
                                                                                           };

    private static void Configure(IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        app.UseCors(x => x //CORS must be called before calling UseEndpoints!   //https://github.com/dotnet/AspNetCore.Docs/pull/21043
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowAnyOrigin());

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapSubscribeHandler();
            endpoints.MapControllers();

            //endpoints.MapControllers().Add(builder=>
            //{
            //    var controllerDesc = builder.Metadata.First(x => x.GetType() == typeof(ControllerActionDescriptor)) as ControllerActionDescriptor;
            //    builder.Metadata.Add(new TopicAttribute(messageBusManager.BusName, controllerDesc.ActionName));
            //});

            //foreach (var subs in messageBusManager.BusSubscriptions)
            //{
            //    endpoints.MapPost(subs.Topic, (Microsoft.AspNetCore.Http.RequestDelegate)subs.Handler).WithTopic(messageBusManager.BusName, subs.Topic);
            //}
        });
        app.UseCloudEvents();
    }
}
