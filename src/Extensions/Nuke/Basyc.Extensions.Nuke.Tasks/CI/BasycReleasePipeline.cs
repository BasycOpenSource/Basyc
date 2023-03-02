using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI;

public class BasycReleasePipeline : BasycPipeline
{
	public BasycReleasePipeline(CiProvider provider, PipelineOs pipelineOs, string[] targets, string[]? importSecrets = null, string[]? importParameters = null)
		: base("release", provider, pipelineOs, new[] { GitFlowBranchType.Develop, GitFlowBranchType.Main }, Trigger.Push, targets, importSecrets, importParameters)
	{
	}
}
