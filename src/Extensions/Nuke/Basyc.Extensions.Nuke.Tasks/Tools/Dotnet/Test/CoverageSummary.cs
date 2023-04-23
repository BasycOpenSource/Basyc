using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public readonly struct CoverageSummary : IDisposable
{
    public CoverageSummary(string summaryDirectory)
    {
        Directory = TemporaryDirectory.CreateFromExisting(summaryDirectory);
    }

    public TemporaryDirectory Directory { get; init; }

    public void Dispose() => Directory.Dispose();
}
