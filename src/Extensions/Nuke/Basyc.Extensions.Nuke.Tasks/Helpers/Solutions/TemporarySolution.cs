using Nuke.Common.ProjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;

[ExcludeFromCodeCoverage]
public readonly struct TemporarySolution : IDisposable
{
	public TemporarySolution(Solution solution)
	{
		Solution = solution;
	}

	public Solution Solution { get; }

	public void Dispose()
	{
		File.Delete(Solution);
	}
}
