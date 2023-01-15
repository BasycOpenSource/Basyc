using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
public readonly struct CoverageSummary : IDisposable
{
	public TemporaryDirectory Directory { get; init; }
	public CoverageSummary(string summaryDirectory)
	{
		Directory = TemporaryDirectory.CreateFromExisting(summaryDirectory);
	}

	public void Dispose()
	{
		Directory.Dispose();
	}
}
