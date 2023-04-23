using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public record CoverageReport(
        TemporaryDirectory Directory,
        ProjectCoverageReport[] Projects)
    : IDisposable
{
    public void Dispose() => Directory.Dispose();
}
