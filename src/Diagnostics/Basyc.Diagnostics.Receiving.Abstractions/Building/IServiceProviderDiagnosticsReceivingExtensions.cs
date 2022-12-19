using Basyc.Diagnostics.Receiving.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceProviderDiagnosticsReceivingExtensions
    {
        public static async Task StartBasycDiagnosticsReceivers(this IServiceProvider serviceProvider)
        {
            var receivers = serviceProvider.GetServices<IDiagnosticReceiver>();
            foreach (var receiver in receivers)
            {
                await receiver.StartReceiving();
            }
        }
    }
}