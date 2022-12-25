using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.ValueInjection;
using Serilog;
using System.Reflection;
using static _build.GitTasks;

namespace Tasks.Git.Diff;

[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class GitCompareReportAttribute : ValueInjectionAttributeBase
{
	public override object GetValue(MemberInfo member, object instance)
	{
		//Workaround();
		var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
		var gitChanges = GitGetCompareReport(repository!.LocalDirectory);
		return gitChanges;
	}

	private object Workaround()
	{
		var repository = Nuke.Common.ControlFlow.SuppressErrors(() => GitRepository.FromLocalDirectory(NukeBuild.RootDirectory));
		if (repository is { Protocol: GitProtocol.Ssh } && !false)
		{
			Log.Warning($"{nameof(GitVersion)} does not support fetching SSH endpoints, enable NoFetch to skip fetching");
		}

		var gitVersion = GitVersionTasks.GitVersion(s => s
				.SetFramework("net5.0")
				.SetNoFetch(false)
				.SetNoCache(false)
				.DisableProcessLogOutput()
				.SetUpdateAssemblyInfo(false)
				.When(TeamCity.Instance is { IsPullRequest: true } && !EnvironmentInfo.Variables.ContainsKey("Git_Branch"), _ => _
						.AddProcessEnvironmentVariable(
							"Git_Branch",
							TeamCity.Instance.ConfigurationProperties.Single(x => x.Key.StartsWith("teamcity.build.vcs.branch")).Value)))
			.Result;

		if (true)
		{
			AzurePipelines.Instance?.UpdateBuildNumber(gitVersion.FullSemVer);
			TeamCity.Instance?.SetBuildNumber(gitVersion.FullSemVer);
			AppVeyor.Instance?.UpdateBuildVersion($"{gitVersion.FullSemVer}.build.{AppVeyor.Instance.BuildNumber}");
		}

		return gitVersion;
	}
}
