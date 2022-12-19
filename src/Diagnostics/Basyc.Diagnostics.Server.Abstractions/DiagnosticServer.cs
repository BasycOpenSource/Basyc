using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Server.Abstractions
{
    public class DiagnosticServer
    {
        private readonly IEnumerable<IServerDiagnosticPusher> pushers;

        public DiagnosticServer(IEnumerable<IServerDiagnosticReceiver> receivers, IEnumerable<IServerDiagnosticPusher> pushers)
        {
            this.pushers = pushers;
            foreach (var receiver in receivers)
            {
                receiver.ChangesReceived += Receiver_ChangesReceived;
            }
        }

        public Task Start()
        {
            return Task.CompletedTask;
        }

        private async void Receiver_ChangesReceived(object? sender, DiagnosticChanges changes)
        {
            foreach (var pusher in pushers)
            {
                await pusher.PushChangesToReceivers(changes);
            }
        }
    }
}