namespace Nuke.Common.CI.GitHubActions;
public static class GithubActionsExtensions
{
	//https://nuget.pkg.github.com/OWNER/index.json
	public static string GetNugetSourceUrl(this GitHubActions gitHubActions)
	{
		return $"https://nuget.pkg.{gitHubActions.ServerUrl.Split("//")[1]}/{gitHubActions.RepositoryOwner}/index.json";
	}
}
