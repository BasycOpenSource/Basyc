using GlobExpressions;
using LibGit2Sharp;
using System.Diagnostics.CodeAnalysis;
using Tasks.Git.Diff;

namespace _build;

public static partial class GitTasks
{
    public static void GitDiff()
    {

    }

    public static GitChangesReport GitGetChangeReport(string localGitFolder, string branchToCompare)
    {
        string newBranchName = Nuke.Common.Tools.Git.GitTasks.GitCurrentBranch();
        string newBranchCommintId = Nuke.Common.Tools.Git.GitTasks.GitCurrentCommit();
        Serilog.Log.Information($"Creating change report between '{newBranchName}' -> '{branchToCompare}'");

        using (var repo = new Repository(localGitFolder))
        {
            var oldBranch = repo.Branches[branchToCompare];
            var newBranch = repo.Branches[newBranchName];
            var newBranchCommit = newBranch.Commits.First(x => x.Id.ToString() == newBranchCommintId);
            var changes = repo.Diff.Compare<TreeChanges>(oldBranch.Tip.Tree, newBranchCommit.Tree);
            List<(string solutionPath, bool solutionChanged, List<string> solutionItems, List<(string projectPath, bool projectChanged, List<string> fileChanges)> projectChanges)> solutionChanges = new();

            string? projectDirectoryRelativePath = null;
            bool projectAlreadyFound = false;

            string? solutionDirectoryRelativePath = null;
            bool solutionAlreadyFound = false;

            string? lastCheckedDirectoryGitRelativePath = null;

            foreach (var change in changes)
            {
                if (change.Exists is false)
                {
                    continue;
                }

                string changeFullPath = Path.Combine(localGitFolder, change.Path);

                if (change.Path.EndsWith(".sln"))
                {
                    bool solutionIsInGitRoot = solutionDirectoryRelativePath == ".\\" && change.Path.IndexOf("/") == -1;
                    if (solutionIsInGitRoot || solutionDirectoryRelativePath == change.Path)
                    {
                        var sol = solutionChanges.Last();
                        sol.solutionChanged = true;
                        continue;
                    }

                    Serilog.Log.Information($"Adding solution because: Solution found. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{change.Path}'. Full change path: '{changeFullPath}'");
                    solutionChanges.Add((changeFullPath, true, new(), new()));
                    solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(change.Path);
                    solutionAlreadyFound = true;
                    continue;
                }

                if (change.Path.EndsWith(".csproj"))
                {
                    if (projectDirectoryRelativePath == GetGitParentDirectoryRelativePath(change.Path))
                    {
                        var (projectPath, projectChanged, fileChanges) = solutionChanges.Last().projectChanges.Last();
                        projectChanged = true;
                        continue;
                    }

                    if (TryGetSolution(changeFullPath, out string? solFullPath) is false)
                    {
                        throw new Exception($"Soltion for project '{changeFullPath}' not found");
                    }

                    string solRelativePath = GetGitRelativePath(solFullPath!, localGitFolder);
                    bool solutionIsInGitRootSameAsLastOne = solutionDirectoryRelativePath == ".\\" && solRelativePath.IndexOf("/") == -1;
                    if (!((solutionAlreadyFound && solutionIsInGitRootSameAsLastOne) || GetGitParentDirectoryRelativePath(solRelativePath) == solutionDirectoryRelativePath))
                    {
                        Serilog.Log.Information($"Adding solution because: Project found. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{change.Path}'. Full change path: '{changeFullPath}'");
                        solutionChanges.Add((solFullPath!, false, new(), new()));
                        solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(solFullPath!, localGitFolder));
                        solutionAlreadyFound = true;
                    }

                    solutionChanges.Last().projectChanges.Add((changeFullPath, true, new()));
                    projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(change.Path);
                    projectAlreadyFound = true;
                    lastCheckedDirectoryGitRelativePath = projectDirectoryRelativePath;
                    continue;
                }

                if (lastCheckedDirectoryGitRelativePath is not null)
                {
                    if (solutionAlreadyFound is false)
                    {
                        throw new NotImplementedException("Unexpected scenario. Items without solution are not supported");
                    }

                    if (projectAlreadyFound is false)
                    {
                        if (TryGetProject(changeFullPath, out string? projectFullPath2))
                        {
                            solutionChanges.Last().projectChanges.Add((projectFullPath2, false, new()));
                            projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(change.Path);
                            projectAlreadyFound = true;
                            lastCheckedDirectoryGitRelativePath = projectDirectoryRelativePath;
                        }
                        else
                        {
                            solutionChanges.Last().solutionItems.Add(changeFullPath);
                            projectDirectoryRelativePath = null;
                            projectAlreadyFound = false;
                            lastCheckedDirectoryGitRelativePath = solutionDirectoryRelativePath;
                            continue;
                        }
                    }

                    if (change.Path.StartsWith(projectDirectoryRelativePath!))
                    {
                        solutionChanges.Last().projectChanges.Last().fileChanges.Add(changeFullPath);
                        continue;
                    }
                    else
                    {
                        lastCheckedDirectoryGitRelativePath = null;
                    }
                }

                if (TryGetSolution(changeFullPath, out string? solFullPath2) is false)
                {
                    throw new Exception($"Soltion for project '{changeFullPath}' not found");
                }

                if (!solutionChanges.Any() || solutionChanges.Last().solutionPath != solFullPath2)
                {
                    Serilog.Log.Information($"Adding solution because: Nothing cached and last solution does not match. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{change.Path}'. Full change path: '{changeFullPath}'");
                    solutionChanges.Add((solFullPath2!, false, new(), new()));
                    solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(solFullPath2!, localGitFolder));
                    solutionAlreadyFound = true;
                }

                if (TryGetProject(changeFullPath, out string? projectFullPath))
                {
                    solutionChanges.Last().projectChanges.Add((projectFullPath, false, new()));
                    solutionChanges.Last().projectChanges.Last().fileChanges.Add(changeFullPath);
                    projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(change.Path);
                    projectAlreadyFound = true;
                    lastCheckedDirectoryGitRelativePath = projectDirectoryRelativePath;
                }
                else
                {
                    solutionChanges.Last().solutionItems.Add(changeFullPath);
                    projectDirectoryRelativePath = null;
                    projectAlreadyFound = false;
                    lastCheckedDirectoryGitRelativePath = solutionDirectoryRelativePath;
                }
            }

            var projectChanges = solutionChanges
                .Select(
                    solutionChanges => new SolutionChanges(solutionChanges.solutionPath, solutionChanges.solutionChanged, solutionChanges.projectChanges
                        .Select(projectChanges => new ProjectChanges(projectChanges.projectPath, projectChanges.projectChanged, projectChanges.fileChanges
                            .Select(fileChange => new FileChange(fileChange))
                            .ToArray()))
                        .ToArray()))
                .ToArray();
            //Serilog.Log.Information("Git changes report created. Changes solutions");

            return new(localGitFolder, projectChanges);
        }
    }

    private static string GetGitParentDirectoryRelativePath(string gitRelativePath)
    {
        string? directoryToSkip;
        var gitRelativePathSpan = gitRelativePath.AsSpan();
        int lastPathSeparator = gitRelativePathSpan.LastIndexOf('/');
        if (lastPathSeparator is -1)
        {
            return ".\\";
        }

        directoryToSkip = gitRelativePathSpan[..lastPathSeparator].ToString();
        return directoryToSkip;
    }

    private static bool TryGetProject(string fullPath, [NotNullWhen(true)] out string? projectPath)
    {

        DirectoryInfo fileDirectory;
        if (IsFile(fullPath))
        {
            if (fullPath.EndsWith(".csproj"))
            {
                projectPath = fullPath;
                return true;
            }

            var file = new FileInfo(fullPath);
            fileDirectory = new DirectoryInfo(file.Directory!.FullName);

        }
        else
        {
            fileDirectory = new DirectoryInfo(fullPath);
        }

        var csprojFile = fileDirectory.GlobFiles("*.csproj").SingleOrDefault();
        if (csprojFile == default)
        {
            var slnFile = fileDirectory.GlobFiles("*.sln").SingleOrDefault();
            if (slnFile != default)
            {
                projectPath = null;
                return false;
            }

            var gitIgnoreFile = fileDirectory.GlobFiles(".gitignore").FirstOrDefault();
            if (gitIgnoreFile != default)
            {
                projectPath = null;
                return false;
            }

            return TryGetProject(fileDirectory.Parent!.FullName, out projectPath);
        }
        else
        {
            projectPath = csprojFile.FullName;
            return true;
        }
    }

    /// <summary>
    /// Returns true if path is file or false if it is directory.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static bool IsFile(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            if (Directory.Exists(path))
            {
                return false;
            }
            else
            {
                throw new ArgumentException($"Path '{path}' is not valid file or directory path", nameof(path));
            }
        }
    }

    private static bool TryGetSolution(string fullPath, out string? solutionFullPath)
    {

        DirectoryInfo fileDirectory;
        if (IsFile(fullPath))
        {
            if (fullPath.EndsWith(".sln"))
            {
                solutionFullPath = fullPath;
                return true;
            }

            var file = new FileInfo(fullPath);
            fileDirectory = new DirectoryInfo(file.Directory!.FullName);

        }
        else
        {
            fileDirectory = new DirectoryInfo(fullPath);
        }

        var solutionFile = fileDirectory.GlobFiles("*.sln").SingleOrDefault();
        if (solutionFile == default)
        {
            var gitIgnoreFile = fileDirectory.GlobFiles(".gitignore").FirstOrDefault();
            if (gitIgnoreFile != default)
            {
                solutionFullPath = null;
                return false;
            }

            return TryGetSolution(fileDirectory.Parent!.FullName, out solutionFullPath);
        }
        else
        {
            solutionFullPath = solutionFile.FullName;
            return true;
        }
    }

    public static string GetGitRelativePath(string filePath, string gitRoot)
    {
        var filePathSpan = filePath.AsSpan();
        var gitRelativePath = filePathSpan[gitRoot.Length..];
        return gitRelativePath.ToString();
    }
}
