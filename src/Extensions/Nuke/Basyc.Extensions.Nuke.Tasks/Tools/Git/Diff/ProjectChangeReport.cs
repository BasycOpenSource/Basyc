namespace Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;

public record ProjectChangeReport(string ProjectFullPath, bool IsProjectChanged, FileChange[] FileChanges)
{
	public string[] GetChangedFilesFullPath()
	{
		return FileChanges
			.Select(x => x.FullPath)
			.Concat(IsProjectChanged ? new[] { ProjectFullPath } : Enumerable.Empty<string>())
			.ToArray();
	}
}
