﻿using Basyc.Extensions.Nuke.CI.GithubActions;
using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

namespace Basyc.Extensions.Nuke.Tasks.CI;

#pragma warning disable CA1813 // Avoid unsealed attributes
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class BasycPipeline : ConfigurationAttributeBase
{
    private readonly ConfigurationAttributeBase baseProvider;

    public BasycPipeline(
        string name,
        CiProvider provider,
        PipelineOs pipelineOs,
        GitFlowBranchType[] gitFlowBranches,
        Trigger trigger,
        string[] targets,
        string[]? importSecrets,
        string[]? importParameters)
    {
        importSecrets ??= Array.Empty<string>();
        importParameters ??= Array.Empty<string>();

        string[] branches = GetBranchesPatterns(gitFlowBranches);
        baseProvider = provider switch
        {
            CiProvider.GithubActions => UseGithub(name, pipelineOs, trigger, branches, targets, importSecrets, importParameters),
            CiProvider.AzurePipelines => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
        Name = name;
        Provider = provider;
        PipelineOs = pipelineOs;
        GitFlowBranches = gitFlowBranches;
        Trigger = trigger;
        Targets = targets;
        ImportSecrets = importSecrets;
        ImportParameters = importParameters;
    }

    public override string IdPostfix => base.IdPostfix + GetType().Name;

    public override Type HostType => baseProvider.HostType;

    public override string ConfigurationFile => baseProvider.ConfigurationFile;

    public override IEnumerable<string> GeneratedFiles => baseProvider.GeneratedFiles;

    public override IEnumerable<string> RelevantTargetNames => baseProvider.RelevantTargetNames;

    public override IEnumerable<string> IrrelevantTargetNames => baseProvider.IrrelevantTargetNames;

    public string Name { get; }

    public CiProvider Provider { get; }

    public PipelineOs PipelineOs { get; }

    public GitFlowBranchType[] GitFlowBranches { get; }

    public Trigger Trigger { get; }

    public string[] Targets { get; }

    public string[]? ImportSecrets { get; }

    public string[]? ImportParameters { get; }

    public override CustomFileWriter CreateWriter(StreamWriter streamWriter) => baseProvider.CreateWriter(streamWriter);

    public override ConfigurationEntity GetConfiguration(NukeBuild build, IReadOnlyCollection<ExecutableTarget> relevantTargets) =>
        baseProvider.GetConfiguration(build, relevantTargets);

    private static string[] GetBranchesPatterns(GitFlowBranchType[] gitFlowBranches)
    {
        var patterns = new List<string>();
        foreach (var branch in gitFlowBranches)
        {
            switch (branch)
            {
                case GitFlowBranchType.Main:
                    patterns.Add("main");
                    break;
                case GitFlowBranchType.HotFix:
                    patterns.Add("hotfix/*");
                    break;
                case GitFlowBranchType.Release:
                    patterns.Add("release/*");
                    break;
                case GitFlowBranchType.Develop:
                    patterns.Add("develop");
                    break;
                case GitFlowBranchType.Feature:
                    patterns.Add("feature/*");
                    break;
                case GitFlowBranchType.PullRequest:
                    break;
                default:
                    break;
            }
        }

        return patterns.ToArray();
    }

    private static ConfigurationAttributeBase UseGithub(
        string name,
        PipelineOs pipelineOs,
        Trigger trigger,
        string[] branches,
        string[] targets,
        string[] importSecrets,
        string[] importParameters)
    {
        var githubImage = pipelineOs == PipelineOs.Windows ? GitHubActionsImage.WindowsLatest : GitHubActionsImage.UbuntuLatest;
        var githubAttribute = new BasycGitHubActionsAttribute(name, githubImage)
        {
            FetchDepth = 0,
            EnableGitHubToken = true,
            ImportSecrets = importSecrets,
            ImportParameters = importParameters
        };
        if (trigger == Trigger.Push)
            githubAttribute.OnPushBranches = branches;
        else
            githubAttribute.OnPullRequestBranches = branches;

        githubAttribute.InvokedTargets = targets;
        return githubAttribute;
    }
}
