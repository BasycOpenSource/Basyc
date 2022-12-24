namespace Tasks.Git.Diff;

public record SolutionChangeReport(string SolutionFullPath, bool IsSolutionChanged, FileChange[] SolutionItemsChanges, ProjectChangeReport[] Projects)
{
	public string[] GetChangedFilesFullPath()
	{
		return Projects
			.SelectMany(x => x.GetChangedFilesFullPath())
			.Concat(SolutionItemsChanges.Select(x => x.FullPath))
			.Concat(IsSolutionChanged ? new[] { SolutionFullPath } : Enumerable.Empty<string>())
			.ToArray();
	}
}