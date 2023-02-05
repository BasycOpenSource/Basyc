namespace Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

public abstract record GitFlowBranch(string Name, GitFlowBranchType BranchType)
{
	public record Main() : GitFlowBranch("main", GitFlowBranchType.Main);

	public record Hotfix(string Name) : GitFlowBranch(Name, GitFlowBranchType.HotFix);

	public record Release(string Name) : GitFlowBranch(Name, GitFlowBranchType.Release);

	public record Develop() : GitFlowBranch("develop", GitFlowBranchType.Develop);

	public record Feature(string Name) : GitFlowBranch(Name, GitFlowBranchType.Feature);

	public record PullRequest(string Name) : GitFlowBranch(Name, GitFlowBranchType.PullRequest);
}
