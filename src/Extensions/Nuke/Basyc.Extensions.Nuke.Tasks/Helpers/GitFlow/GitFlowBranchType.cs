namespace Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

public enum GitFlowBranchType
{
	Main,
	HotFix,
	Release,
	Develop,
	Feature
}

public abstract record GitFlowBranches(string Name, GitFlowBranchType BranchType)
{
	public record Main() : GitFlowBranches("main", GitFlowBranchType.Main);

	public record Hotfix(string Name) : GitFlowBranches(Name, GitFlowBranchType.HotFix);

	public record Release(string Name) : GitFlowBranches(Name, GitFlowBranchType.Release);

	public record Develop() : GitFlowBranches("develop", GitFlowBranchType.Develop);

	public record Feature(string Name) : GitFlowBranches(Name, GitFlowBranchType.Feature);
}
