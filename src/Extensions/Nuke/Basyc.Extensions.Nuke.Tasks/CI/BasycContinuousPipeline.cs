using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI
{
	public class BasycContinuousPipeline : BasycPipelineAttribute
	{
		public BasycContinuousPipeline(CiProviders provider, HostOs hostOs, string[] targets)
			: base("continuous", provider, hostOs, new[] { GitFlowBranches.Feature, GitFlowBranches.Release, GitFlowBranches.HotFix }, Trigger.Push, targets)
		{
		}
	}
}
