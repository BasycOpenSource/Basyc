using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.CI;

#pragma warning disable CA1813 // Avoid unsealed attributes
public class BasycContinuousPipeline : BasycPipeline
{
    public BasycContinuousPipeline(CiProvider provider, PipelineOs pipelineOs, string[] targets, string[]? importSecrets = null, string[]? importParameters = null)
        : base("continuous", provider, pipelineOs, new[] { GitFlowBranchType.Feature, GitFlowBranchType.Release, GitFlowBranchType.HotFix }, Trigger.Push, targets, importSecrets, importParameters)
    {
    }
}
