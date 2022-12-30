using Nuke.Common.Git;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class GitTasks
{
	public static void GitCreateTag(string tagName, GitRepository gitRepository)
	{
		//Git($"config --global user.email \"build@ourcompany.com\""); 
		//Git($"config --global user.name \"Our Company Build\"");
		//Git($"tag -a {GitVersion.FullSemVer} -m \"Setting git tag on commit to '{GitVersion.FullSemVer}'\"");
		//Git($"push https://{AzurePipelines.Instance.AccessToken}@dev.azure.com/<companyname>/{AzurePipelines.Instance.TeamProject}/_git/{AzurePipelines.Instance.RepositoryName} --tags");
		global::Nuke.Common.Tools.Git.GitTasks.Git($"config --global user.email \"bot@dummyemail.com\"");
		global::Nuke.Common.Tools.Git.GitTasks.Git($"config --global user.name \"Automated Bot\"");
		global::Nuke.Common.Tools.Git.GitTasks.Git($"tag -a {tagName} -m \"Setting git tag on commit to '{tagName}'\"");
		global::Nuke.Common.Tools.Git.GitTasks.Git($"push  --tags");
	}
}
