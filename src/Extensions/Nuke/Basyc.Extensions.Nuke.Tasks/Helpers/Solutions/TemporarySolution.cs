using Nuke.Common.ProjectModel;

namespace Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
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
