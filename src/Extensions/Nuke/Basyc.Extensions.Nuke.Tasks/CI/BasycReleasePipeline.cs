using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI;

public class BasycReleasePipeline : BasycPipelineAttribute
{
	public BasycReleasePipeline(CiProvider provider, HostOs hostOs, string[] targets)
		: base("release", provider, hostOs, new[] { GitFlowBranches.Develop, GitFlowBranches.Main }, Trigger.Push, targets)
	{
	}
}
