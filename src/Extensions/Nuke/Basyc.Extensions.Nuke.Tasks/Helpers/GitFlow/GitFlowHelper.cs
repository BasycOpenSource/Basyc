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


	/// <param name="branchName"></param>
	/// <returns>Branch from which provided branch should be based on according the GitFlow</returns>
	/// <exception cref="StringNotGitFlowBranch"></exception>
	public static GitFlowBranch GetSourceBranch(string branchName)
	{
		if (branchName.IsFeatureBranch())
			return new GitFlowBranch.Develop();

		if (branchName.IsDevelopBranch())
			return new GitFlowBranch.Main();

		if (branchName.IsReleaseBranch())
			return new GitFlowBranch.Develop();

		if (branchName.IsHotfixBranch())
			return new GitFlowBranch.Release(branchName);

		throw new StringNotGitFlowBranch($"Can't determine source branch for '{branchName}'");
	}

	public static GitFlowBranch GetBranch(string branchName)
	{
		if (branchName.IsMainBranch())
			return new GitFlowBranch.Main();

		if (branchName.IsDevelopBranch())
			return new GitFlowBranch.Develop();

		if (branchName.IsFeatureBranch())
			return new GitFlowBranch.Feature(branchName);

		if (branchName.IsReleaseBranch())
			return new GitFlowBranch.Release(branchName);

		if (branchName.IsHotfixBranch())
			return new GitFlowBranch.Hotfix(branchName);

		if (branchName.IsPullRequestBranch())
			return new GitFlowBranch.PullRequest(branchName);

		throw new StringNotGitFlowBranch($"Can't create branch for branch name '{branchName}'");
	}

	public static bool IsGitFlowBranch(string branch)
	{
		try
		{
			GetBranch(branch);
		}
		catch (StringNotGitFlowBranch)
		{
			return false;
		}

		return true;
	}
}
