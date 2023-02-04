using LibGit2Sharp;
using Nuke.Common;
using Nuke.Common.Git;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static partial class GitTasks
{
	public static void Commit(string message)
	{
		var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
		using var repo = new Repository(repository.LocalDirectory);
		var author = new Signature("Nuke pipeline", "dummy@email.com", DateTime.Now);
		var committer = author;
		repo.Commit(message, author, committer);
	}

	public static void Push()
	{
		var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
		using var repo = new Repository(repository.LocalDirectory);
		var options = new PushOptions();
		options.CredentialsProvider = (url, usernameFromUrl, types) =>
		{
			// var credentials = new UsernamePasswordCredentials
			// {
			// 	Username = USERNAME,
			// 	Password = PASSWORD
			// };
			var credentials = new DefaultCredentials();
			return credentials;
		};
		repo.Network.Push(repo.Branches[repository.Branch], options);
	}
}
