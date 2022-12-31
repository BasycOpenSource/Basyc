namespace Nuke.Common.CI.GitHubActions;
public static class GithubActionsBasycExtensions
{
	/// <summary>
	/// Returns nuget feed url in format: https://nuget.pkg.github.com/<OWNER>/index.json
	/// </summary>
	/// <param name="gitHubActions"></param>
	/// <returns></returns>
	public static string GetNugetSourceUrl(this GitHubActions gitHubActions)
	{
		return $"https://nuget.pkg.{gitHubActions.ServerUrl.Split("//")[1]}/{gitHubActions.RepositoryOwner}/index.json";
	}

	public static string GetPullRequestTargetBranch(this GitHubActions gitHubActions)
	{
		//var pullRequestObject = gitHubActions.GitHubEvent.GetPropertyValue("pull_request");
		//string targetBranch = pullRequestObject["base"]!.Value<string>("ref");
		//return targetBranch;
		return gitHubActions.BaseRef;
	}

	public static string GetPullRequestSourceBranch(this GitHubActions gitHubActions)
	{
		return gitHubActions.HeadRef;
	}
}
