namespace Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

public static class GitFlowHelper
{
	public static bool IsPullRequestAllowed(string sourceBranch, string targetBranch, bool canSkipReleaseBranch = false)
	{
		if (sourceBranch.IsMainOrMasterBranch())
			return false;

		if (sourceBranch.IsDevelopBranch())
		{
			if (targetBranch.IsReleaseBranch())
				return true;

			if (canSkipReleaseBranch)
				if (targetBranch.IsMainOrMasterBranch())
					return true;

			return false;
		}

		if (sourceBranch.IsReleaseBranch())
			return targetBranch.IsMainOrMasterBranch() || targetBranch.IsDevelopBranch();

		if (sourceBranch.IsFeatureBranch())
			return targetBranch.IsDevelopBranch() || targetBranch.IsReleaseBranch();

		if (sourceBranch.IsHotfixBranch())
			return targetBranch.IsMainOrMasterBranch() || targetBranch.IsDevelopBranch();

		throw new InvalidOperationException($"Failed to check '{sourceBranch}' against '{targetBranch}'");
	}

	public static bool IsReleaseAllowed(string currentBranch)
	{
		return currentBranch.IsMainOrMasterBranch() || currentBranch.IsDevelopBranch();
	}

	public static GitFlowBranchType GetSourceBranchType(string branch)
	{
		if (branch.IsFeatureBranch())
			return GitFlowBranchType.Develop;

		if (branch.IsDevelopBranch())
			return GitFlowBranchType.Main;

		if (branch.IsReleaseBranch())
			return GitFlowBranchType.Develop;

		if (branch.IsHotfixBranch())
			return GitFlowBranchType.Release;

		throw new StringNotGitFlowBranch($"Cant determine source branch for '{branch}'");
	}

	public static bool IsGitFlowBranch(string branch)
	{
		try
		{
			GetSourceBranchType(branch);
		}
		catch (StringNotGitFlowBranch)
		{
			return false;
		}

		return true;
	}
}
