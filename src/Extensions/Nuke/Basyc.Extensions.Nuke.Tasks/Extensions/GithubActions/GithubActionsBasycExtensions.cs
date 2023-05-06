using System.Diagnostics.CodeAnalysis;

namespace Nuke.Common.CI.GitHubActions;

[ExcludeFromCodeCoverage]
public static class GithubActionsBasycExtensions
{
    /// <summary>
    ///     Returns nuget feed url in format: https://nuget.pkg.github.com/[OWNER]/index.json.
    /// </summary>
    public static Uri GetNugetSourceUrl(this GitHubActions gitHubActions) =>
        new($"https://nuget.pkg.{gitHubActions.ServerUrl.Split("//")[1]}/{gitHubActions.RepositoryOwner}/index.json");

    public static string GetPullRequestTargetBranch(this GitHubActions gitHubActions) => gitHubActions.BaseRef;

    public static string GetPullRequestSourceBranch(this GitHubActions gitHubActions) => gitHubActions.HeadRef;

    public static bool IsPullRequest(this GitHubActions? gitHubActions) => gitHubActions is not null && gitHubActions.IsPullRequest;
}
