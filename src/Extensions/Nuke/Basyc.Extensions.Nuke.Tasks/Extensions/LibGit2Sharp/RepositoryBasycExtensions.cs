using LibGit2Sharp;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.Extensions.Nuke.Tasks.Extensions.LibGit2Sharp;

[ExcludeFromCodeCoverage]
internal static class RepositoryBasycExtensions
{
    public static bool HasUncommitedChanges(this Repository repo)
    {
        var giStatus = repo.RetrieveStatus();
        var hasUncommitedChanges = giStatus.Any();
        return hasUncommitedChanges;
    }

    public static IEnumerable<string> GetUncommitedRemovedChanges(this Repository repo)
    {
        var gitStatus = repo.RetrieveStatus();
        var uncommitedChanges = gitStatus.Missing.Concat(gitStatus.Removed);
        return uncommitedChanges.Select(x => x.FilePath);
    }

    public static IEnumerable<string> GetUncommitedChanges(this Repository repo)
    {
        var gitStatus = repo.RetrieveStatus();
        var uncommitedChanges = gitStatus.Untracked
            .Concat(gitStatus.Modified)
            .Concat(gitStatus.Added)
            .Concat(gitStatus.RenamedInIndex)
            .Concat(gitStatus.RenamedInWorkDir);

        return uncommitedChanges.Select(x => x.FilePath);
    }

    public static Commit GetFirstSharedCommit(this Repository repo, Branch oldBranch, Branch newBranch)
    {
        var filter = new CommitFilter
        {
            ExcludeReachableFrom = oldBranch,
            IncludeReachableFrom = newBranch
        };
        var newBranchFirstCommit = repo.Commits.QueryBy(filter).LastOrDefault();
        if (newBranchFirstCommit == default)
        {
            //If new branch does not contain any commit we check if old branch last commit (excluding merge commit) is new branch last commit
            var newBranchLastCommit = newBranch.Commits.First();
            var oldBranchLastCommit = oldBranch.Commits.First();
            if (oldBranchLastCommit.Parents.Contains(newBranchLastCommit) is false)
                throw new InvalidOperationException("Could not find first shared commit");

            return newBranchLastCommit;
        }

        return newBranchFirstCommit.Parents.First();
    }
}
