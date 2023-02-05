using Nuke.Common.Utilities;
using System.IO.Enumeration;

namespace Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

public static class GitFlowBranchStringExtensions
{
	public static bool IsMainOrMasterBranch(this string branch)
	{
		return branch.IsMainBranch() ||
				branch.IsMasterBranch();
	}

	public static bool IsMasterBranch(this string branch)
	{
		return branch?.EqualsOrdinalIgnoreCase("master") ?? false;
	}

	public static bool IsMainBranch(this string branch)
	{
		return branch?.EqualsOrdinalIgnoreCase("main") ?? false;
	}

	public static bool IsDevelopBranch(this string branch)
	{
		return (branch?.EqualsOrdinalIgnoreCase("dev") ?? false) ||
				(branch?.EqualsOrdinalIgnoreCase("develop") ?? false) ||
				(branch?.EqualsOrdinalIgnoreCase("development") ?? false);
	}

	public static bool IsFeatureBranch(this string branch)
	{
		return (branch?.StartsWithOrdinalIgnoreCase("feature/") ?? false) ||
				(branch?.StartsWithOrdinalIgnoreCase("features/") ?? false);
	}

	// public static bool IsOnBugfixBranch(this string branch)
	// {
	//     return branch?.StartsWithOrdinalIgnoreCase("feature/fix-") ?? false;
	// }

	public static bool IsReleaseBranch(this string branch)
	{
		return (branch?.StartsWithOrdinalIgnoreCase("release/") ?? false) ||
				(branch?.StartsWithOrdinalIgnoreCase("releases/") ?? false);
	}

	public static bool IsHotfixBranch(this string branch)
	{
		return (branch?.StartsWithOrdinalIgnoreCase("hotfix/") ?? false) ||
				(branch?.StartsWithOrdinalIgnoreCase("hotfixes/") ?? false);
	}

	public static bool IsPullRequestBranch(this string branch)
	{
		bool isMatch = FileSystemName.MatchesSimpleExpression("*pull/*/merge", branch);
		return isMatch;
	}
}
