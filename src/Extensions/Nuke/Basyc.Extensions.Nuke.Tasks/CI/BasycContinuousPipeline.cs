using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI;

public class BasycContinuousPipeline : BasycPipeline
{
	public BasycContinuousPipeline(CiProvider provider, PipelineOs pipelineOs, string[] targets, string[]? importSecrets = null)
		: base("continuous", provider, pipelineOs, new[] { GitFlowBranchType.Feature, GitFlowBranchType.Release, GitFlowBranchType.HotFix }, Trigger.Push, targets, importSecrets)
	{
	}
}
