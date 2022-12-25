namespace Basyc.Extensions.Nuke.Tasks.Git.Diff;

public record GitCompareReport(string GitRepoLocalDirectory, bool CouldCompare, SolutionChangeReport[] ChangedSolutions);
