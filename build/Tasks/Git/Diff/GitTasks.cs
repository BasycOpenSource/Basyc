
using GlobExpressions;
using LibGit2Sharp;

namespace _build;

public static partial class GitTasks
{
    public static void GitDiff()
    {

    }


    public static string[] GitGetChangedProjects(string localGitFolder, string branchToCompare)
    {
        var currentBranch = Nuke.Common.Tools.Git.GitTasks.GitCurrentBranch();
        var currentCommit = Nuke.Common.Tools.Git.GitTasks.GitCurrentCommit();

        using (var repo = new Repository(localGitFolder))
        {
            var main = repo.Branches[branchToCompare];
            var currentBranchObject = repo.Branches[currentBranch];
            var currentCommitObject = currentBranchObject.Commits.First(x => x.Id.ToString() == currentCommit);
            var changes = repo.Diff.Compare<TreeChanges>(main.Tip.Tree, currentCommitObject.Tree);
            HashSet<string> changedProjects = new();
            foreach (var change in changes)
            {

                var changedProject = GetProjectFilePathForFile(change.Path);
                if (changedProject is not null)
                    changedProjects.Add(changedProject);
            }
            return changedProjects.ToArray();
        }
    }

    /// <summary>
    /// Returns null if file does not have a csproj file (for example solution items)
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static string? GetProjectFilePathForFile(string filePath)
    {
        var file = new FileInfo(filePath);
        var fileDirectory = new DirectoryInfo(file.Directory!.FullName);
        var csprojFile = fileDirectory.GlobFiles("*.csproj").SingleOrDefault();
        if (csprojFile == default)
        {
            var slnFile = fileDirectory.GlobFiles("*.sln").SingleOrDefault();
            if (slnFile != default)
                return null;

            return GetProjectFilePathForFile(fileDirectory.Parent!.FullName);
        }
        else
        {
            return csprojFile.FullName;
        }
    }
}
