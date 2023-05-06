using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct CoverageSummary : IDisposable
{
    public CoverageSummary(string summaryDirectory)
    {
        Directory = TemporaryDirectory.CreateFromExisting(summaryDirectory);
    }

    public TemporaryDirectory Directory { get; init; }

    public void Dispose() => Directory.Dispose();
}
