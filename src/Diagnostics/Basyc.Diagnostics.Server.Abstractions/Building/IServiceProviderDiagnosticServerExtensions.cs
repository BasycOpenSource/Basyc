using Basyc.Diagnostics.Server.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceProviderDiagnosticServerExtensions
    {
        public static Task StartBasycDiagnosticServer(this IServiceProvider serviceProvider)
        {
            var server = serviceProvider.GetRequiredService<DiagnosticServer>();
            return server.Start();
        }

    }
}