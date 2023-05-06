using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Basyc.Extensions.Nuke.Tasks.Tools.Structure;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using Serilog;

namespace Basyc.Extensions.Nuke.Targets;

public interface IBasycBuildBase : INukeBuild
{
    string BuildProjectName { get; }

    UnitTestSettings UnitTestSettings { get; }

    PullRequestSettings PullRequestSettings { get; }

    Solution Solution { get; }

    [GitVersion]
    GitVersion GitVersion => TryGetValue(() => GitVersion)!;

    [GitRepository]
    GitRepository GitRepository => TryGetValue(() => GitRepository)!;

    [Repository]
    Repository Repository => TryGetValue(() => Repository)!;

    protected void BranchCheck()
    {
        if (GitFlowHelper.IsGitFlowBranch(GitRepository.Branch!) is false)
        {
            throw new InvalidOperationException(
                $"Branch '{GitRepository.Branch!}' is not allowed branch name according git flow");
        }

        Log.Information($"Branch name '{GitRepository.Branch!}' is valid git flow branch");
    }

    protected void PullRequestCheck()
    {
        PullRequestSettings.AsserIfValid();

        if (PullRequestSettings.IsPullRequest is false)
            throw new InvalidOperationException("Can't perform pull request check if this run is not marked as pull request");

        if (GitFlowHelper.IsPullRequestAllowed(PullRequestSettings.SourceBranch!, PullRequestSettings.TargetBranch!) is false)
        {
            throw new InvalidOperationException(
                $"Pull request between {PullRequestSettings.SourceBranch!} and {PullRequestSettings.TargetBranch!} is not allowed according git flow");
        }

        Log.Information($"Pull request between '{PullRequestSettings.SourceBranch!}' and '{PullRequestSettings.TargetBranch!}' and is valid according git flow");
    }
}
