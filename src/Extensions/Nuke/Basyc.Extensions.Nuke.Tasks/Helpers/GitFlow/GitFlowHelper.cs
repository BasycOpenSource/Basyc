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
			{
				if (targetBranch.IsMainOrMasterBranch())
					return true;
			}

			return false;
		}

		if (sourceBranch.IsReleaseBranch())
		{
			if (targetBranch.IsMainOrMasterBranch())
				return true;

			if (targetBranch.IsDevelopBranch())
				return true;

			return false;
		}

		if (sourceBranch.IsFeatureBranch())
		{
			if (targetBranch.IsDevelopBranch())
				return true;

			if (targetBranch.IsReleaseBranch())
				return true;

			return false;
		}

		if (sourceBranch.IsHotfixBranch())
		{
			if (targetBranch.IsMainOrMasterBranch())
				return true;

			if (targetBranch.IsDevelopBranch())
				return true;

			return false;
		}

		throw new InvalidOperationException($"Failed to check '{sourceBranch}' against '{targetBranch}'");
	}

	public static bool IsReleaseAllowed(string currentBranch)
	{
		if (currentBranch.IsMainOrMasterBranch() || currentBranch.IsDevelopBranch())
			return true;

		return false;
	}

	public static GitFlowBranches GetGitFlowSourceBranch(string currentBranch)
	{
		if (currentBranch.IsFeatureBranch())
			return GitFlowBranches.Develop;

		if (currentBranch.IsDevelopBranch())
			return GitFlowBranches.Main;

		if (currentBranch.IsReleaseBranch())
			return GitFlowBranches.Develop;

		if (currentBranch.IsHotfixBranch())
			return GitFlowBranches.Release;

		throw new ArgumentException(nameof(currentBranch));
	}
}
