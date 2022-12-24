namespace Tasks.Git.Diff;

public record GitCompareReport(string GitRepoLocalDirectory, bool CouldCompare, SolutionChangeReport[] Solutions);
