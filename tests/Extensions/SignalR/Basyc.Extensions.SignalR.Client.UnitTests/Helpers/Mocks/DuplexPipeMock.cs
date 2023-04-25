using System.IO.Pipelines;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

public class DuplexPipeMock : IDuplexPipe
{
    public DuplexPipeMock(Pipe pipe)
    {
        this.Pipe = pipe;
    }

    public Pipe Pipe { get; init; }

    public PipeReader Input => Pipe.Reader;

    public PipeWriter Output => Pipe.Writer;
}
