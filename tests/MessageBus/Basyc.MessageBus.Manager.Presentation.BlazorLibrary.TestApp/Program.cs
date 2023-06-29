using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Receiving.Abstractions;
using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Helpers;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.DomainDrivenDesign.Domain;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.Manager;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Basyc.MessageBus.Manager.Infrastructure.Building.Diagnostics;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Building;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;
using Basyc.Shared.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;
#pragma warning disable CA2254 // Template should be a static expression

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var assembliesToScan = new[] { typeof(TestCommand).Assembly };

//builder.Services.AddLogging(x =>
//{
//    x.AddDebug();
//});

builder.Logging.AddDebug();
builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBasycDiagnosticsExporting()
    .SetDefaultIdentity("BusManager")
    //.AddSignalRExporter()
    .AddInMemoryExporter()
    .ListenFor()
    .AnyActvity()
    .AnyLog();

builder.Services.AddBasycDiagnosticsReceiving()
    .AddInMemoryReceiver();
//.SelectSignalRReceiver()
//.SetServerUri("https://localhost:44310");

builder.Services.AddBasycMessageBus()
    .RegisterHandlersFromAssembly<TestCommandHandler>()
    //.NoHandlers()
    //.SelectSignalRProxyProvider("https://localhost:44310")
    .SelectNullClient()
    .EnableDiagnostics();

var busManagerBuilder = builder.Services.AddBasycBusManager();

busManagerBuilder.EnableDiagnostics()
    .AddBasycDiagnostics();

busManagerBuilder.AddRequestHandler()
    .UseTraceIdMapper<BusManagerBasycDiagnosticsReceiverTraceIdMapper>()
    .UseBasycMessageBusHandler();

// busManagerBuilder.RegisterMessages()
//  .FromAssemblyScan(assembliesToScan)
//  .InGroup("FromAssembly")
//  .FromInterface<IEvent>()
//  .UseTypeNameAsDisplayName()
//  .AsEvents()
//  .HandledByDefaultHandler();
//
// busManagerBuilder.RegisterMessages()
//  .FromAssemblyScan(assembliesToScan)
//  .InGroup("FromAssembly")
//  .FromInterface<ICommand>()
//  .UseTypeNameAsDisplayName()
//  .AsCommands()
//  .NoResponse()
//  .HandledBy(BasycTypedMessageBusRequestHandler.BasycTypedMessageBusRequesterUniqueName);
//
// busManagerBuilder.RegisterMessages()
//  .FromAssemblyScan(assembliesToScan)
//  .InGroup("FromAssembly")
//  .FromInterface(typeof(ICommand<>))
//  .UseTypeNameAsDisplayName()
//  .AsQueries()
//  .HasResponse<int>()
//  .SetResponseDisplayName("responseType");
//
// busManagerBuilder.RegisterMessages()
//  .FromAssemblyScan(assembliesToScan)
//  .InGroup("FromAssembly")
//  .FromInterface(typeof(IQuery<>))
//  .UseTypeNameAsDisplayName()
//  .AsQueries()
//  .HasResponse<int>()
//  .SetResponseDisplayName("asddas")
//  .HandledByDefaultHandler();
IDiagnosticsExporter? diagnsoticExporter = null;
busManagerBuilder.RegisterMessages()
    .FromAssemblyScan(assembliesToScan)
    .WhereImplements<ICommand>()
    .Register((type, register) =>
    {
        register.InGroup("FromAssembly")
            .AddMessage(type.Name)
            .NoReturn()
            .HandledBy((logger) =>
            {
                //var activity = DiagnosticHelper.Start("Handler logic");
                //activity.Stop();
                //x.Complete();
            });
    });

busManagerBuilder.RegisterMessages()
    .FromAssemblyScan(assembliesToScan)
    .WhereImplements(typeof(IQuery<>))
    .Register((type, register) =>
    {
        register.InGroup("FromAssembly")
            .AddMessage(type.Name)
            .WithParametersFrom(type)
            .Returns(type.GetTypeArgumentsFromParent(typeof(IQuery<>)).First())
            .HandledBy((x, logger) =>
            {
                string text = (string)x.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).First().GetValue(x).Value();
                return text.ToUpperInvariant();
            });
    });

busManagerBuilder.RegisterMessages()
    .FromFluentApi()
    .InGroup("FromFluentApi")
    .AddMessage("Fluent Message 1")
    .NoReturn()
    .HandledBy(logger =>
    {
        if (Random.Shared.Next(0, 100) > 50)
        {
            Thread.Sleep(100);
            logger.LogError("TestError");
            throw new InvalidOperationException("Test exception");
        }
    })
    .AddMessage("ToUpper")
    .WithParameter<string>("name")
    .Returns<string>("nameToUpper")
    .HandledBy((x, logger) =>
    {
        string? name = (string)x.First().Value.Value();
        return name.ToUpperInvariant();
    })
    .AddMessage("ToLower")
    .WithParameter<string>("name")
    .Returns<string>("nameToLower")
    .HandledBy((x, logger) =>
    {
        string? name = (string)x.First().Value.Value();
        return name.ToUpperInvariant();
    })
    .AddMessage("Add Customer")
    .WithParametersFrom<CustomerModel>()
    .Returns<string>("errorCode")
    .HandeledBy((CustomerModel x) =>
    {
        return "0";
    })
    .AddMessage("Create Customer")
    .WithParametersFrom<CustomerModel>()
    .Returns<CustomerModel>("new customer")
    .HandeledBy((CustomerModel x) =>
    {
        return x;
    })
    .AddMessage("Infinite Logging")
    .WithParameter<int>("log start count")
    .WithParameter<bool>("only errors")
    .NoReturn()
    .HandledBy(async (s, logger) =>
    {
        var initCount = (int)s.Parameters[0].Value.Value();
        var onlyErrors = (bool)s.Parameters[1].Value.Value();
        for (int i = 0; i < initCount; i++)
        {
            if (onlyErrors is false)
                logger.LogInformation("Info: " + i++);
            logger.LogError("Error: " + i++);
        }

        int logCounter = initCount;
        while (true)
        {
            await Task.Delay(3500);
            if (onlyErrors is false)
                logger.LogInformation("Info: " + logCounter++);
            logger.LogError("Error: " + logCounter++);
        }
    })
    .AddMessage("Multiple Services")
    .NoReturn()
    .HandledBy((logger) =>
    {
        void SeedActivityData(ServiceIdentity serviceIdentity)
        {
            Random.Shared.Next(8, 20).Times(x =>
            {
                var traceId = DiagnosticHelper.GetCurrentTraceId();
                var activityStart = new ActivityStart(serviceIdentity, traceId, null, IdGeneratorHelper.GenerateNewSpanId(), serviceIdentity.ServiceName, DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds((x * 10) + Random.Shared.Next(8, 20)));
                diagnsoticExporter.Value().StartActivity(activityStart);
                diagnsoticExporter.Value().EndActivity(activityStart, activityStart.StartTime + TimeSpan.FromMilliseconds(x * 1.2));
                var logEntry = new LogEntry(serviceIdentity, traceId, DateTimeOffset.UtcNow, LogLevel.Information, "Message", null);
                diagnsoticExporter.Value().ProduceLog(logEntry);
            });
        }

        void SeedServiceData(string serviceName)
        {
            var serviceIdentity = new ServiceIdentity(serviceName);
            SeedActivityData(serviceIdentity);
        }

        SeedServiceData("ServiceA");
        SeedServiceData("ServiceB");
        SeedServiceData("ServiceC");
        SeedServiceData("ServiceD");
        SeedServiceData("ServiceF");
    })
    .AddMessage("Log")
    .WithParameter<int>("logCount")
    .NoReturn()
    .HandledBy((input, logger) =>
    {
        int logCounter = 0;
        int desiredCount = (int)input.Parameters.First().Value.Value();
        while (true)
        {
            logger.LogInformation("Info: " + logCounter++);
            logger.LogError("Error: " + logCounter++);
            if (logCounter >= desiredCount)
                break;
        }
    });
#pragma warning disable CA1416 // Validate platform compatibility
builder.Services.AddBasycBusManagerBlazorUi();
#pragma warning restore CA1416 // Validate platform compatibility
var blazorApp = builder.Build();
diagnsoticExporter = blazorApp.Services.GetRequiredService<IDiagnosticsExporter>();
WireUpInMemoryDiagnostics(blazorApp);
await blazorApp.Services.StartBasycDiagnosticsReceivers();
await blazorApp.Services.StartBasycDiagnosticExporters();
await blazorApp.Services.StartBasycMessageBusClient();
await blazorApp.RunAsync();

static void WireUpInMemoryDiagnostics(WebAssemblyHost app)
{
    var serverReceiver = app.Services.GetRequiredService<InMemoryDiagnosticReceiver>();
    var inMemoryProducer = app.Services.GetRequiredService<InMemoryDiagnosticsExporter>();
    inMemoryProducer.LogProduced += (s, a) =>
    {
        serverReceiver.PushLog(a);
    };

    inMemoryProducer.StartProduced += (s, a) =>
    {
        serverReceiver.StartActivity(a);
    };

    inMemoryProducer.EndProduced += (s, a) =>
    {
        serverReceiver.EndActivity(a);
    };
}
