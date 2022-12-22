
namespace Tasks.Git.Diff
{
    public record ProjectChanges(string ProjectFullPath, bool IsProjectChanged, FileChange[] FileChanges)
    {
        public string[] GetAllChangedFiles()
        {
            return FileChanges
                .Select(x => x.FilePath)
                .Concat(IsProjectChanged ? new[] { ProjectFullPath } : Enumerable.Empty<string>())
                .ToArray();
        }
    }
}