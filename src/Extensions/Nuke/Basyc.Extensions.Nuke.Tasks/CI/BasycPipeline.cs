using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

namespace Basyc.Extensions.Nuke.Tasks.CI;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class BasycPipelineAttribute : ConfigurationAttributeBase
{
	private readonly ConfigurationAttributeBase baseProvider;
	public BasycPipelineAttribute(string name, CiProviders provider, HostOs hostOs, GitFlowBranches[] gitFlowBranches, Trigger trigger, string[] targets)
	{
		string[] branches = GetBranchesPatterns(gitFlowBranches);
		baseProvider = provider switch
		{
			CiProviders.GithubActions => UseGithub(name, hostOs, trigger, branches, targets),
			CiProviders.AzurePipelines => throw new NotImplementedException(),
			_ => throw new NotImplementedException(),
		};
	}

	private static string[] GetBranchesPatterns(GitFlowBranches[] gitFlowBranches)
	{
		var patterns = new List<string>();
		foreach (var branch in gitFlowBranches)
		{
			switch (branch)
			{
				case GitFlowBranches.Main:
					patterns.Add("main");
					break;
				case GitFlowBranches.HotFix:
					patterns.Add("hotfix/*");
					break;
				case GitFlowBranches.Release:
					patterns.Add("release/*");
					break;
				case GitFlowBranches.Develop:
					patterns.Add("develop");
					break;
				case GitFlowBranches.Feature:
					patterns.Add("feature/*");
					break;
				default:
					break;
			}
		}

		return patterns.ToArray();
	}

	private static ConfigurationAttributeBase UseGithub(string name, HostOs hostOs, Trigger trigger, string[] branches, string[] targets)
	{
		var githubImage = hostOs == HostOs.Windows ? GitHubActionsImage.WindowsLatest : GitHubActionsImage.UbuntuLatest;
		var githubAttribute = new GitHubActionsAttribute(name, githubImage)
		{
			FetchDepth = 0,
			EnableGitHubToken = true
		};
		if (trigger == Trigger.Push)
			githubAttribute.OnPushBranches = branches;
		else
			githubAttribute.OnPullRequestBranches = branches;
		githubAttribute.InvokedTargets = targets;
		return githubAttribute;
	}

	public override string IdPostfix => base.IdPostfix + GetType().Name;

	public override Type HostType => baseProvider.HostType;
	public override string ConfigurationFile => baseProvider.ConfigurationFile;
	public override IEnumerable<string> GeneratedFiles => baseProvider.GeneratedFiles;
	public override IEnumerable<string> RelevantTargetNames => baseProvider.RelevantTargetNames;
	public override IEnumerable<string> IrrelevantTargetNames => baseProvider.IrrelevantTargetNames;

	public override CustomFileWriter CreateWriter(StreamWriter streamWriter)
	{
		return baseProvider.CreateWriter(streamWriter);
	}

	public override ConfigurationEntity GetConfiguration(NukeBuild build, IReadOnlyCollection<ExecutableTarget> relevantTargets)
	{
		return baseProvider.GetConfiguration(build, relevantTargets);
	}
}
