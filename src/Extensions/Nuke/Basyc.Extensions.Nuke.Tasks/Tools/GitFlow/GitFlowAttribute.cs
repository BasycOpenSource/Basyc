using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ValueInjection;
using System.Reflection;

namespace Basyc.Extensions.Nuke.Tasks.Tools.GitFlow;

public sealed class GitFlowAttribute : ValueInjectionAttributeBase
{
    public override object GetValue(MemberInfo member, object instance)
    {
        var gitRepo = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
        var branch = GitFlowHelper.GetBranch(gitRepo.Branch!);
        return new GitFlow(branch);
    }
}
