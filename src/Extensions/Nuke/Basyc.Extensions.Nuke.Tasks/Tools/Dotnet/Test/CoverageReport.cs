using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
#pragma warning disable CA1819 // Properties should not return arrays

public record CoverageReport(
        TemporaryDirectory Directory,
        ProjectCoverageReport[] Projects)
    : IDisposable
{
    public void Dispose() => Directory.Dispose();
}
