using System.Diagnostics.CodeAnalysis;

namespace Nuke.Common.CI.GitHubActions;

[ExcludeFromCodeCoverage]
public static class GithubActionsBasycExtensions
{
	/// <summary>
	///     Returns nuget feed url in format: https://nuget.pkg.github.com/<OWNER>/index.json
	/// </summary>
	/// <param name="gitHubActions"></param>
	/// <returns></returns>
	public static string GetNugetSourceUrl(this GitHubActions gitHubActions)
	{
		return $"https://nuget.pkg.{gitHubActions.ServerUrl.Split("//")[1]}/{gitHubActions.RepositoryOwner}/index.json";
	}

	public static string GetPullRequestTargetBranch(this GitHubActions gitHubActions)
	{
		return gitHubActions.BaseRef;
	}

	public static string GetPullRequestSourceBranch(this GitHubActions gitHubActions)
	{
		return gitHubActions.HeadRef;
	}

	public static bool IsPullRequest(this GitHubActions? gitHubActions)
	{
		return gitHubActions is not null && gitHubActions.IsPullRequest;
	}
}
