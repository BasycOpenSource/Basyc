using System.IO.Pipelines;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks
{
    public class DuplexPipeMock : IDuplexPipe
    {
        public readonly Pipe pipe;

        public DuplexPipeMock(Pipe pipe)
        {
            this.pipe = pipe;
        }
        public PipeReader Input => pipe.Reader;

        public PipeWriter Output => pipe.Writer;
    }
}