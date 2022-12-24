namespace Tasks.Git.Diff;

public record SolutionChanges(string SolutionFullPath, bool IsSolutionChanged, FileChange[] SolutionItemsChanges, ProjectChanges[] ProjectChanges)
{
    public string[] GetChangedFilesFullPath()
    {
        //return ProjectChanges
        //	.SelectMany(x => x.FileChanges)
        //	.Select(x => x.FilePath)
        //	.Concat(ProjectChanges.Where(x => x.IsProjectChanged).Select(x => x.ProjectFullPath))
        //	.Concat(IsSolutionChanged ? new[] { SolutionFullPath } : Enumerable.Empty<string>())
        //	.ToArray();

        return ProjectChanges
            .SelectMany(x => x.GetChangedFilesFullPath())
            .Concat(SolutionItemsChanges.Select(x => x.FullPath))
            .Concat(IsSolutionChanged ? new[] { SolutionFullPath } : Enumerable.Empty<string>())
            .ToArray();
    }
}