namespace Tasks.Git.Diff;

public record SolutionChangeReport(string SolutionFullPath, bool IsSolutionChanged, FileChange[] SolutionItemsChanges, ProjectChangeReport[] ProjectChanges)
{
	public string[] GetChangedFilesFullPath()
	{
		return ProjectChanges
			.SelectMany(x => x.GetChangedFilesFullPath())
			.Concat(SolutionItemsChanges.Select(x => x.FullPath))
			.Concat(IsSolutionChanged ? new[] { SolutionFullPath } : Enumerable.Empty<string>())
			.ToArray();
	}
}