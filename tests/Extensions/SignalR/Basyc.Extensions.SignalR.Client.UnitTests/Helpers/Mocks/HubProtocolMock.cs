using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks
{
    public class HubProtocolMock : IHubProtocol
    {
        private Queue<HubProtocolMockMessage> mockMessages { get; set; } = new();

        public HubProtocolMock()
        {
        }
        public string Name => nameof(HubProtocolMock);

        public int Version => 1;

        public TransferFormat TransferFormat => throw new NotImplementedException();

        public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
        {
            throw new NotImplementedException();
        }

        public bool IsVersionSupported(int version) => true;

        public bool TryParseMessage(ref ReadOnlySequence<byte> input, IInvocationBinder binder, [NotNullWhen(true)] out HubMessage? message)
        {
            if (mockMessages.TryDequeue(out var getterMessage) is false)
            {
                message = null;
                return false;
            }

            //binder.GetParameterTypes()
            InvocationMessage? invocationMessage = new InvocationMessage(getterMessage.Target, getterMessage.Arguments);
            message = invocationMessage;
            return true;
        }

        public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
        {

        }

        public void AddReceivingMessage(HubProtocolMockMessage message)
        {
            mockMessages.Enqueue(message);
        }

    }
}