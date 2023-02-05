namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static partial class GitTasks
{
	public static async Task<GitCredentials> GetCredentialsWindows()
	{
		// var tt = global::Nuke.Common.Tools.Git.GitTasks.Git("");
		return await GitCliWrapper.GetCredentialsWindows();
	}
}
