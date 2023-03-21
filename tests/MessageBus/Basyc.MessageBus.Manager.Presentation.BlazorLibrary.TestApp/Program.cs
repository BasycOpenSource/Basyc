using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Receiving.Abstractions;
using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.DomainDrivenDesign.Domain;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.Manager;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Basyc.MessageBus.Manager.Infrastructure.Building.Diagnostics;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Building;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;
using Basyc.MessageBus.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Diagnostics;
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
	.AddGroup("FromFluentApi")
	.AddMessage("Fluent Message 1")
	.NoReturn()
	.HandledBy((MessageRequest x) =>
	{
		var activity = DiagnosticHelper.Start("Handler logic");
		activity.Stop();
		if (Random.Shared.Next(0, 100) > 50)
		{
			var serviceA = new ServiceIdentity("ServiceA");
			x.Diagnostics.AddLog(ServiceIdentity.ApplicationWideIdentity, LogLevel.Error, "TestError", null);
			x.Diagnostics.AddLog(serviceA, LogLevel.Information, "Test", null);
			var ac = x.Diagnostics.AddStartActivity(new ActivityStart(serviceA, x.Diagnostics.TraceId, null, "1", "ServiceAStart", DateTimeOffset.UtcNow));
			Thread.Sleep(100);
			x.Diagnostics.AddLog(serviceA, LogLevel.Information, "Test2", null);

			//x.Diagnostics.AddEndActivity(new ActivityEnd(serviceA, x.Diagnostics.TraceId, null,"2", "ServiceAStart", DateTimeOffset.UtcNow, ));
			ac.End(DateTimeOffset.UtcNow, ActivityStatusCode.Ok);
			x.Fail("Because");
			return;
		}
		x.Complete();

	})
	.AddMessage("Fluent Message 2")
	.NoReturn()
	.HandledBy((MessageRequest x) =>
	{
		x.Diagnostics.AddLog(ServiceIdentity.ApplicationWideIdentity, LogLevel.Error, "TestError", null);
		x.Complete();
	});

builder.Services.AddBasycBusManagerBlazorUi();
//builder.Services.UseMicrosoftDependencyResolver(); //Splat config
//var resolver = Locator.CurrentMutable;
//resolver.InitializeSplat();
//resolver.InitializeReactiveUI();

//Locator.CurrentMutable.Register(() => new SidebarHistory(), typeof(IViewFor<SidebarHistoryViewModel>)); //Splat!

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
