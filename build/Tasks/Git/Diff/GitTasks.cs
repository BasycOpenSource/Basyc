
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
        var newBranchName = Nuke.Common.Tools.Git.GitTasks.GitCurrentBranch();
        var newBranchCommintId = Nuke.Common.Tools.Git.GitTasks.GitCurrentCommit();

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

            string? lastCheckedDirectory = null;

            foreach (var change in changes)
            {
                if (change.Exists is false)
                    continue;

                string changeFullPath = $"{localGitFolder}\\{change.Path.Replace("/", "\\")}";

                if (change.Path.EndsWith(".sln"))
                {
                    bool solutionIsInGitRoot = (solutionDirectoryRelativePath == ".\\" && change.Path.IndexOf("/") == -1);
                    if (solutionIsInGitRoot || solutionDirectoryRelativePath == change.Path)
                    {
                        var sol = solutionChanges.Last();
                        sol.solutionChanged = true;
                        continue;
                    }
                    solutionChanges.Add((changeFullPath, true, new(), new()));
                    solutionDirectoryRelativePath = GetParentDirectoryRelativePath(change.Path);
                    solutionAlreadyFound = true;
                    continue;
                }

                if (change.Path.EndsWith(".csproj"))
                {
                    if (projectDirectoryRelativePath == GetParentDirectoryRelativePath(change.Path))
                    {
                        var proj = solutionChanges.Last().projectChanges.Last();
                        proj.projectChanged = true;
                        continue;
                    }

                    if (TryGetSolution(changeFullPath, out var solFullPath) is false)
                    {
                        throw new Exception($"Soltion for project '{changeFullPath}' not found");
                    }

                    string solRelativePath = GetGitRelativePath(solFullPath!, localGitFolder);
                    bool solutionIsInGitRootSameAsLastOne = (solutionDirectoryRelativePath == ".\\" && solRelativePath.IndexOf("/") == -1);
                    if (solutionAlreadyFound && solutionIsInGitRootSameAsLastOne || solRelativePath == solutionDirectoryRelativePath)
                    {

                    }
                    else
                    {
                        solutionChanges.Add((solFullPath!, false, new(), new()));
                        solutionDirectoryRelativePath = GetParentDirectoryRelativePath(GetGitRelativePath(solFullPath!, localGitFolder));
                        solutionAlreadyFound = true;
                    }
                    solutionChanges.Last().projectChanges.Add((changeFullPath, true, new()));
                    projectDirectoryRelativePath = GetParentDirectoryRelativePath(change.Path);
                    projectAlreadyFound = true;
                    lastCheckedDirectory = projectDirectoryRelativePath;
                    continue;
                }

                if (lastCheckedDirectory is not null)
                {
                    if (solutionAlreadyFound is false)
                        throw new NotImplementedException("Unexpected scenario. Items without solution are not supported");

                    if (projectAlreadyFound is false)
                    {
                        if (TryGetProject(changeFullPath, out var projectFullPath2))
                        {
                            solutionChanges.Last().projectChanges.Add((projectFullPath2, false, new()));
                            projectDirectoryRelativePath = GetParentDirectoryRelativePath(change.Path);
                            projectAlreadyFound = true;
                            lastCheckedDirectory = projectDirectoryRelativePath;
                        }
                        else
                        {
                            solutionChanges.Last().solutionItems.Add(changeFullPath);
                            projectDirectoryRelativePath = null;
                            projectAlreadyFound = false;
                            lastCheckedDirectory = solutionDirectoryRelativePath;
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
                        lastCheckedDirectory = null;
                    }
                }



                if (TryGetSolution(changeFullPath, out var solFullPath2) is false)
                {
                    throw new Exception($"Soltion for proejct '{changeFullPath}' not found");
                }

                if (solutionChanges.Any() && solutionChanges.Last().solutionPath == solFullPath2)
                {

                }
                else
                {
                    solutionChanges.Add((solFullPath2!, false, new(), new()));
                    solutionDirectoryRelativePath = GetParentDirectoryRelativePath(GetGitRelativePath(solFullPath2!, localGitFolder));
                    solutionAlreadyFound = true;
                }


                if (TryGetProject(changeFullPath, out var projectFullPath))
                {
                    solutionChanges.Last().projectChanges.Add((projectFullPath, false, new()));
                    solutionChanges.Last().projectChanges.Last().fileChanges.Add(changeFullPath);
                    projectDirectoryRelativePath = GetParentDirectoryRelativePath(change.Path);
                    projectAlreadyFound = true;
                    lastCheckedDirectory = projectDirectoryRelativePath;
                }
                else
                {
                    solutionChanges.Last().solutionItems.Add(changeFullPath);
                    projectDirectoryRelativePath = null;
                    projectAlreadyFound = false;
                    lastCheckedDirectory = solutionDirectoryRelativePath;
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
            return new(localGitFolder, projectChanges);
        }
    }

    private static string GetParentDirectoryRelativePath(string relativePath)
    {
        string? directoryToSkip;
        var relativPathSpan = relativePath.AsSpan();
        var lastPathSeparator = relativPathSpan.LastIndexOf('/');
        if (lastPathSeparator is -1)
        {
            return ".\\";
        }
        directoryToSkip = relativPathSpan.Slice(0, lastPathSeparator).ToString();
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
                throw new ArgumentException("Invalid path. Path is not file or directory", nameof(path));
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
        var gitRelativePath = filePathSpan.Slice(gitRoot.Length);
        return gitRelativePath.ToString();
    }

}