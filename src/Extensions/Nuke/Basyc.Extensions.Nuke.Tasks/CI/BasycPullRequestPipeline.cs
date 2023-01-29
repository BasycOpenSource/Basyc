using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI;

public class BasycPullRequestPipeline : BasycPipelineAttribute
{
	public BasycPullRequestPipeline(CiProvider provider, HostOs hostOs, string[] targets)
		: base("pullRequest", provider, hostOs, new[] { GitFlowBranches.Develop, GitFlowBranches.Main }, Trigger.PullRequest, targets)
	{
	}
}
