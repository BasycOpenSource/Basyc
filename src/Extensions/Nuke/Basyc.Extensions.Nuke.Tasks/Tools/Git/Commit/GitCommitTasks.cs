using LibGit2Sharp;
using Nuke.Common;
using Nuke.Common.Git;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static partial class GitTasks
{
    public static void Commit(string message, params string[] filesToCommit)
    {
        var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
        filesToCommit = filesToCommit
            .Select(x => GetGitRelativePath(x, repository.LocalDirectory!))
            .ToArray();
        using var repo = new Repository(repository.LocalDirectory);
        var author = new Signature("PipelineBot", "pipelineBot@pipelineBot.com", DateTime.Now);
        var committer = author;
        foreach (string fileToCommit in filesToCommit)
        {
            repo.Index.Add(fileToCommit);
            repo.Index.Write();
        }

        repo.Commit(message, author, committer);
    }

    public static void Push(GitCredentials gitCredentials)
    {
        var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
        using var repo = new Repository(repository.LocalDirectory);
        var options = new PushOptions();
        options.CredentialsProvider = (url, usernameFromUrl, types) =>
        {
            var credentials = new UsernamePasswordCredentials
            {
                Username = gitCredentials.UserName,
                Password = gitCredentials.Password
            };
            return credentials;
        };
        repo.Network.Push(repo.Branches[repository.Branch], options);
        // global::Nuke.Common.Tools.Git.GitTasks.Git("push");
    }
}
