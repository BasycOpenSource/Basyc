using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI;

public class BasycPullRequestPipeline : BasycPipeline
{
	public BasycPullRequestPipeline(CiProvider provider, PipelineOs pipelineOs, string[] targets)
		: base("pullRequest", provider, pipelineOs, new[] { GitFlowBranchType.Develop, GitFlowBranchType.Main }, Trigger.PullRequest, targets)
	{
	}
}
