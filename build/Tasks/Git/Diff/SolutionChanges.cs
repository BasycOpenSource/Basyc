namespace Tasks.Git.Diff
{
    public record SolutionChanges(string SolutionFullPath, bool IsSolutionChanged, ProjectChanges[] ProjectChanges)
    {
        public string[] GetAllChangedFiles()
        {
            return ProjectChanges
                .SelectMany(x => x.FileChanges)
                .Select(x => x.FilePath)
                .Concat(ProjectChanges.Where(x => x.IsProjectChanged).Select(x => x.ProjectFullPath))
                .Concat(IsSolutionChanged ? new[] { SolutionFullPath } : Enumerable.Empty<string>())
                .ToArray();
        }
    }
}