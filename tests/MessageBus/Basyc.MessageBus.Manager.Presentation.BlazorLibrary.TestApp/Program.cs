using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Receiving.Abstractions;
using Basyc.Diagnostics.Shared;
using Basyc.DomainDrivenDesign.Domain;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.Manager;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Basyc.MessageBus.Manager.Infrastructure.Building.Diagnostics;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;
using Basyc.MessageBus.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ICommand = System.Windows.Input.ICommand;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var assembliesToScan = new[] { typeof(TestCommand).Assembly };

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBasycDiagnosticExporting()
	.SetDefaultIdentity("BusManager")
	//.AddSignalRExporter()
	.AddInMemoryExporter()
	.ListenFor()
	.AnyActvity()
	.AnyLog();

builder.Services.AddBasycDiagnosticReceiving()
	.AddInMemoryReceiver();
// 	.SelectSignalRReceiver()
// 	.SetServerUri("https://localhost:44310");

builder.Services.AddBasycMessageBus()
	.NoHandlers()
	//.SelectSignalRProxyProvider("https://localhost:44310")
	.SelectNullClient()
	.EnableDiagnostics();


var busManagerBuilder = builder.Services.AddBasycBusManager();

busManagerBuilder.EnableDiagnostics()
	.AddBasycDiagnostics();

busManagerBuilder.AddRequestHandler()
	.UseTraceIdMapper<BusManagerBasycDiagnosticsReceiverTraceIdMapper>()
	.UseBasycMessageBusHandler();

busManagerBuilder.RegisterMessages()
	.FromAssembly(assembliesToScan)
	.InGroup("FromAssembly")
	.FromInterface<IEvent>()
	.UseTypeNameAsDisplayName()
	.AsEvents()
	.HandledByDefaultHandler();

busManagerBuilder.RegisterMessages()
	.FromAssembly(assembliesToScan)
	.InGroup("FromAssembly")
	.FromInterface<ICommand>()
	.UseTypeNameAsDisplayName()
	.AsCommands()
	.NoResponse()
	.HandledByDefaultHandler();

busManagerBuilder.RegisterMessages()
	.FromAssembly(assembliesToScan)
	.InGroup("FromAssembly")
	.FromInterface(typeof(ICommand<>))
	.UseTypeNameAsDisplayName()
	.AsQueries()
	.HasResponse<int>()
	.SetResponseDisplayName("responseType")
	.HandledByDefaultHandler();


busManagerBuilder.RegisterMessages()
	.FromAssembly(assembliesToScan)
	.InGroup("FromAssembly")
	.FromInterface(typeof(IQuery<>))
	.UseTypeNameAsDisplayName()
	.AsQueries()
	.HasResponse<int>()
	.SetResponseDisplayName("asddas")
	.HandledByDefaultHandler();

busManagerBuilder.RegisterMessages()
	.FromFluentApi()
	.InGroup("FromFluentApi")
	.AddMessage("Fluent Message 1", RequestType.Command)
	.NoReturn()
	.HandledBy(x =>
	{
		var activity = DiagnosticHelper.Start("Handler logic");
		activity.Stop();
		x.Complete();
	});

builder.Services.AddBasycBusManagerBlazorUi();


//CreateTestingMessages(busManagerBuilder);

var blazorApp = builder.Build();
WireUpInMemoryProducers(blazorApp);
await blazorApp.Services.StartBasycDiagnosticsReceivers();
await blazorApp.Services.StartBasycDiagnosticExporters();
await blazorApp.Services.StartBasycMessageBusClient();
await blazorApp.RunAsync();

static void WireUpInMemoryProducers(WebAssemblyHost app)
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
