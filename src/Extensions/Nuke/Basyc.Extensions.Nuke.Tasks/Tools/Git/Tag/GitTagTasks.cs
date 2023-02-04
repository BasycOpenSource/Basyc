using Nuke.Common.Git;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static partial class GitTasks
{
	public static void GitCreateTag(string tagName, GitRepository gitRepository)
	{
		global::Nuke.Common.Tools.Git.GitTasks.Git("config --global user.email \"bot@dummyemail.com\"");
		global::Nuke.Common.Tools.Git.GitTasks.Git("config --global user.name \"Automated Bot\"");
		global::Nuke.Common.Tools.Git.GitTasks.Git($"tag -a {tagName} -m \"Setting git tag on commit to '{tagName}'\"");
		global::Nuke.Common.Tools.Git.GitTasks.Git("push  --tags");
	}
}
