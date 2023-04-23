using System.Diagnostics.CodeAnalysis;
using Basyc.Extensions.Nuke.Tasks.Extensions.LibGit2Sharp;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using GlobExpressions;
using LibGit2Sharp;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static partial class GitTasks
{
    private const string gitRoot = ".\\";

    private const string remoteName = "origin";

    private const string solutionExtension = ".sln";

    private const string projectExtension = ".csproj";

    //ProjectModelTasks.Initialize(); //https://github.com/nuke-build/nuke/issues/844
    public static RepositoryChangeReport GitGetAffectedReport(string localGitFolder, string? oldBranchName = null)
    {
        localGitFolder = localGitFolder.Replace("\\", "/");
        if (oldBranchName == null)
        {
            if (TryGetBranchToCompareName(out oldBranchName) is false)
                return new(localGitFolder, false, Array.Empty<SolutionChangeReport>());
        }

        using var repo = new Repository(localGitFolder);
        GetBranchesToCompare(repo, oldBranchName, out var newBranchLocal, out var oldBranchLocal);
        Log.Information(
            $"Creating change report between '{newBranchLocal.FriendlyName}:{newBranchLocal.Tip.Id.ToString().Substring(0, 6)}:{newBranchLocal.Tip.MessageShort}' -> '{newBranchLocal.FriendlyName}:{oldBranchLocal.Tip.Id.ToString().Substring(0, 6)}:{oldBranchLocal.Tip.MessageShort}'");
        List<(string SolutionPath, bool SolutionChanged, List<string> SolutionItems, List<(string ProjectPath, bool ProjectChanged, List<string> FileChanges)>
            ProjectChanges)> solutionChanges = new();
        var commitNewBranchBasedOn = repo.GetFirstSharedCommit(oldBranchLocal, newBranchLocal);
        var paths = repo.Diff.Compare<TreeChanges>(commitNewBranchBasedOn.Tree, newBranchLocal.Tip.Tree)
            .Where(x => x.Exists)
            .Select(x => x.Path);

        SortedSet<string> changesGitRelativePaths = new(paths);

        if (repo.HasUncommitedChanges())
        {
            repo.GetUncommitedRemovedChanges().ForEach(x => changesGitRelativePaths.Remove(x));
            repo.GetUncommitedChanges().ForEach(x => changesGitRelativePaths.Add(x));
        }

        string? projectDirectoryRelativePath = null;
        bool projectAlreadyFound = false;
        string? solutionDirectoryRelativePath = null;
        bool solutionAlreadyFound = false;
        string? lastCheckedDirectoryGitRelativePath = null;

        foreach (string changeRelativePath in changesGitRelativePaths)
        {
            string changeFullPath = $"{localGitFolder}/{changeRelativePath}";

            if (changeRelativePath.EndsWith(solutionExtension))
            {
                bool isChangeInGitRoot = !changeRelativePath.Contains('/', StringComparison.CurrentCulture);
                bool solutionIsInGitRoot = solutionDirectoryRelativePath == gitRoot && isChangeInGitRoot;
                string changeParentDir = GetGitParentDirectoryRelativePath(changeRelativePath);
                if (solutionIsInGitRoot || solutionDirectoryRelativePath == changeParentDir)
                {
                    var sol = solutionChanges.Last();
                    sol.SolutionChanged = true;
                    continue;
                }

                Log.Debug(
                    $"Adding solution because: Solution found. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{changeRelativePath}'. Full change path: '{changeFullPath}'");
                solutionChanges.Add((changeFullPath, true, new(), new()));
                solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(changeRelativePath);
                solutionAlreadyFound = true;
                continue;
            }

            if (changeRelativePath.EndsWith(projectExtension))
            {
                if (projectDirectoryRelativePath == GetGitParentDirectoryRelativePath(changeRelativePath))
                {
                    (string projectPath, bool projectChanged, var fileChanges) = solutionChanges.Last().ProjectChanges.Last();
                    projectChanged = true;
                    continue;
                }

                string solutionFilePath = GetSolution(changeFullPath);

                string solRelativePath = GetGitRelativePath(solutionFilePath!, localGitFolder);
                bool solutionIsInGitRootSameAsLastOne =
                    solutionDirectoryRelativePath == gitRoot && !solRelativePath.Contains('/', StringComparison.CurrentCulture);
                if (!((solutionAlreadyFound && solutionIsInGitRootSameAsLastOne) ||
                      GetGitParentDirectoryRelativePath(solRelativePath) == solutionDirectoryRelativePath))
                {
                    Log.Debug(
                        $"Adding solution because: Project found. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{changeRelativePath}'. Full change path: '{changeFullPath}'");
                    solutionChanges.Add((solutionFilePath!, false, new(),
                        new()));
                    solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(solutionFilePath!, localGitFolder));
                    solutionAlreadyFound = true;
                }

                solutionChanges.Last().ProjectChanges.Add((changeFullPath, true, new()));
                projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(changeRelativePath);
                projectAlreadyFound = true;
                lastCheckedDirectoryGitRelativePath = projectDirectoryRelativePath;
                continue;
            }

            if (lastCheckedDirectoryGitRelativePath is not null)
            {
                if (solutionAlreadyFound is false)
                    throw new InvalidOperationException("Unexpected scenario. Items without solution are not supported");

                if (projectAlreadyFound is false)
                {
                    if (TryGetProject(changeFullPath, out string? projectFullPath2))
                    {
                        solutionChanges.Last().ProjectChanges.Add((projectFullPath2, false, new()));
                        projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(projectFullPath2, localGitFolder));
                        projectAlreadyFound = true;
                        lastCheckedDirectoryGitRelativePath = projectDirectoryRelativePath;
                    }
                    else
                    {
                        solutionChanges.Last().SolutionItems.Add(changeFullPath);
                        projectDirectoryRelativePath = null;
                        projectAlreadyFound = false;
                        lastCheckedDirectoryGitRelativePath = solutionDirectoryRelativePath;
                        continue;
                    }
                }

                if (changeRelativePath.StartsWith(projectDirectoryRelativePath!))
                {
                    solutionChanges.Last().ProjectChanges.Last().FileChanges.Add(changeFullPath);
                    continue;
                }

                lastCheckedDirectoryGitRelativePath = null;
            }

            string solutionFilePath2 = GetSolution(changeFullPath);

            if (!solutionChanges.Any() || solutionChanges.Last().SolutionPath != solutionFilePath2)
            {
                Log.Debug(
                    $"Adding solution because: Nothing cached and last solution does not match. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{changeRelativePath}'. Full change path: '{changeFullPath}'");
                solutionChanges.Add((solutionFilePath2!, false, new(), new()));
                solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(solutionFilePath2!, localGitFolder));
                solutionAlreadyFound = true;
            }

            if (TryGetProject(changeFullPath, out string? projectFullPath))
            {
                solutionChanges.Last().ProjectChanges.Add((projectFullPath, false, new()));
                solutionChanges.Last().ProjectChanges.Last().FileChanges.Add(changeFullPath);
                projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(projectFullPath, localGitFolder));
                projectAlreadyFound = true;
                lastCheckedDirectoryGitRelativePath = projectDirectoryRelativePath;
            }
            else
            {
                solutionChanges.Last().SolutionItems.Add(changeFullPath);
                projectDirectoryRelativePath = null;
                projectAlreadyFound = false;
                lastCheckedDirectoryGitRelativePath = solutionDirectoryRelativePath;
            }
        }
#pragma warning disable SA1117
        var projectChanges = solutionChanges
            .Select(
                solutionChanges => new SolutionChangeReport(solutionChanges.SolutionPath, solutionChanges.SolutionChanged, solutionChanges.SolutionItems
                    .Select(filePath => new FileChange(filePath))
                    .ToArray(), solutionChanges.ProjectChanges
                    .Select(projectChanges => new ProjectChangeReport(projectChanges.ProjectPath, projectChanges.ProjectChanged, projectChanges.FileChanges
                        .Select(filePath => new FileChange(filePath))
                        .ToArray()))
                    .ToArray()))
            .ToArray();
#pragma warning restore SA1117

        var report = new RepositoryChangeReport(localGitFolder, true, projectChanges);
        LogReport(report);
        return report;
    }

    private static void LogReport(RepositoryChangeReport report)
    {
        Log.Debug("Added or modified files:");
        foreach (var solution in report.ChangedSolutions)
        {
            Log.Debug($"  Solution: {solution.SolutionFullPath}");
            foreach (var solutionItem in solution.SolutionItemsChanges)
                Log.Debug($"    Solution item: {solutionItem.FullPath}");

            foreach (var project in solution.ChangedProjects)
            {
                Log.Debug($"    Project: {project.ProjectFullPath}");
                foreach (var file in project.FileChanges)
                    Log.Debug($"      File: {file.FullPath}");
            }
        }
    }

    private static void GetBranchesToCompare(Repository repo, string? oldBranchName, out Branch newBranchLocal, out Branch oldBranchLocal)
    {
        string newBranchName = GitCurrentBranch();
        string newBranchCommintId = GitCurrentCommit();
        newBranchLocal = repo.Branches[newBranchName];
        var oldBranchRemote = repo.Branches[$"{remoteName}/{oldBranchName}"];
        var remoteSource = repo.Network.Remotes[remoteName];
        string log = string.Empty;
        Commands.Fetch(repo, remoteSource.Name, new[] { $"{oldBranchName}:{oldBranchName}" }, new(), log);
        oldBranchLocal = repo.Branches[oldBranchName];
    }

    private static string GetGitParentDirectoryRelativePath(string gitRelativePath)
    {
        string? directoryToSkip;
        var gitRelativePathSpan = gitRelativePath.AsSpan();
        int lastPathSeparator = gitRelativePathSpan.LastIndexOf('/');

        if (lastPathSeparator is -1)
            return gitRoot;

        directoryToSkip = gitRelativePathSpan[..lastPathSeparator].ToString();
        return directoryToSkip;
    }

    private static bool TryGetProject(string fullPath, [NotNullWhen(true)] out string? projectPath)
    {
        DirectoryInfo fileDirectory;
        if (IsFile(fullPath))
        {
            if (fullPath.EndsWith(projectExtension))
            {
                projectPath = fullPath;
                return true;
            }

            var file = new FileInfo(fullPath);
            fileDirectory = new(file.Directory!.FullName);
        }
        else
        {
            fileDirectory = new(fullPath);
        }

        var csprojFile = fileDirectory.GlobFiles($"*{projectExtension}").SingleOrDefault();
        if (csprojFile == default)
        {
            var slnFile = fileDirectory.GlobFiles($"*{solutionExtension}").FirstOrDefault();
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

        projectPath = csprojFile.FullName.Replace('\\', '/');
        return true;
    }

    /// <summary>
    ///     Returns true if path is file or false if it is directory.
    /// </summary>
    private static bool IsFile(string path)
    {
        if (File.Exists(path))
            return true;
        if (Directory.Exists(path))
            return false;
        throw new ArgumentException($"Path '{path}' does not exists or is not valid file or directory path", nameof(path));
    }

    private static string GetSolution(string fullPath)
    {
        if (TryGetSolution(fullPath, out string? solutionFullPath) is false)
            throw new ArgumentException($"Solution for file '{fullPath}' not found");

        return solutionFullPath!;
    }

    private static bool TryGetSolution(string fullPath, out string? solutionFullPath)
    {
        DirectoryInfo fileDirectory;
        if (IsFile(fullPath))
        {
            if (fullPath.EndsWith(solutionExtension))
            {
                solutionFullPath = fullPath;
                return true;
            }

            var file = new FileInfo(fullPath);
            fileDirectory = new(file.Directory!.FullName);
        }
        else
        {
            fileDirectory = new(fullPath);
        }

        var solutionFile = fileDirectory.GlobFiles($"*{solutionExtension}").SingleOrDefault();
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

        solutionFullPath = solutionFile.FullName.Replace('\\', '/');
        return true;
    }

    private static string GetGitRelativePath(string filePath, string gitRoot)
    {
        var filePathSpan = filePath.AsSpan();
        var gitRelativePath = filePathSpan.Slice(gitRoot.Length + 1);
        return gitRelativePath.ToString();
    }

    private static bool TryGetBranchToCompareName(out string? branchToCompareName)
    {
        var gitRepoNuke = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
        if (gitRepoNuke.IsOnMainBranch())
        {
            branchToCompareName = null;
            return false;
        }

        if (gitRepoNuke.IsOnDevelopBranch())
        {
            branchToCompareName = "main";
            return true;
        }

        if (gitRepoNuke.IsOnFeatureBranch())
        {
            branchToCompareName = "develop";
            return true;
        }

        if (gitRepoNuke.IsOnReleaseBranch())
        {
            branchToCompareName = "main";
            return true;
        }

        if (gitRepoNuke.IsOnHotfixBranch())
        {
            branchToCompareName = "main";
            return true;
        }

        branchToCompareName = null;
        return false;
    }
}
