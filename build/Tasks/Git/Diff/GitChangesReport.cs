namespace Tasks.Git.Diff;

public record GitChangesReport(string GitRepoLocalDirectory, SolutionChanges[] SolutionChanges);