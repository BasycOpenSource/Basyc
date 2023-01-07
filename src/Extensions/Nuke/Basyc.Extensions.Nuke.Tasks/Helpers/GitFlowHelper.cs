using System.Diagnostics.CodeAnalysis;

namespace Basyc.Extensions.Nuke.Tasks.Helpers;

[ExcludeFromCodeCoverage]
public static class GitFlowHelper
{
	public static bool IsPullRequestAllowed(string sourceBranch, string targetBranch, bool canSkipReleaseBranch)
	{
		sourceBranch = sourceBranch.ToLowerInvariant();
		targetBranch = targetBranch.ToLowerInvariant();

		if (sourceBranch is "main" or "master")
		{
			return false;
		}

		if (sourceBranch is "develop")
		{
			if (targetBranch.StartsWith("release"))
			{
				return true;
			}

			if (canSkipReleaseBranch)
			{
				if (targetBranch is "main" or "master")
				{
					return true;
				}
			}

			return false;
		}

		if (sourceBranch.StartsWith("release"))
		{
			if (targetBranch is "main" or "master")
			{
				return true;
			}

			if (targetBranch is "develop")
			{
				return true;
			}

			return false;
		}

		if (sourceBranch.StartsWith("feature"))
		{
			if (targetBranch is "develop")
			{
				return true;
			}

			if (targetBranch.StartsWith("release"))
			{
				return true;
			}

			return false;
		}

		if (sourceBranch.StartsWith("hotfix"))
		{
			if (targetBranch is "main" or "master")
			{
				return true;
			}

			if (targetBranch is "develop")
			{
				return true;
			}

			return false;
		}

		throw new InvalidOperationException($"Failed to check '{sourceBranch}' against '{targetBranch}'");
	}

	public static bool IsReleaseAllowed(string currentBranch)
	{
		if (currentBranch is "main" or "master" or "develop")
		{
			return true;
		}

		return false;
	}
}
