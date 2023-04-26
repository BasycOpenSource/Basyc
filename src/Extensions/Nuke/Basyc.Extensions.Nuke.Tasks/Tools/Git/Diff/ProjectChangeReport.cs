namespace Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;

#pragma warning disable CA1819 // Properties should not return arrays
public record ProjectChangeReport(string ProjectFullPath, bool IsProjectChanged, FileChange[] FileChanges)
{
    public string[] GetChangedFilesFullPath() => FileChanges
            .Select(x => x.FullPath)
            .Concat(IsProjectChanged ? new[] { ProjectFullPath } : Enumerable.Empty<string>())
            .ToArray();
}
