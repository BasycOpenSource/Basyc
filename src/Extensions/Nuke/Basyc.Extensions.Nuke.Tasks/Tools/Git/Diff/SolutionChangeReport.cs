﻿namespace Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;

public record SolutionChangeReport(string SolutionFullPath, bool IsSolutionChanged, FileChange[] SolutionItemsChanges, ProjectChangeReport[] ChangedProjects)
{
    public string[] GetChangedFilesFullPath() => ChangedProjects
            .SelectMany(x => x.GetChangedFilesFullPath())
            .Concat(SolutionItemsChanges.Select(x => x.FullPath))
            .Concat(IsSolutionChanged ? new[] { SolutionFullPath } : Enumerable.Empty<string>())
            .ToArray();
}
