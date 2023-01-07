using LibGit2Sharp;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.Extensions.Nuke.Tasks.Extensions.LibGit2Sharp;

[ExcludeFromCodeCoverage]
internal static class RepositoryBasycExtensions
{
	public static bool HasUncommitedChanges(this Repository repo)
	{
		var giStatus = repo.RetrieveStatus();
		bool hasUncommitedChanges = giStatus.Any();
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
}
