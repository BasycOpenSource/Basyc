using Basyc.Extensions.Nuke.Tasks.Git.Diff;
using GlobExpressions;
using LibGit2Sharp;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Basyc.Extensions.Nuke.Tasks;

public static partial class GitTasks
{
	private const string gitRoot = ".\\";
	private const string remoteName = "origin";
	private const string solutionExtension = ".sln";
	private const string projectExtension = ".csproj";

	//ProjectModelTasks.Initialize(); //https://github.com/nuke-build/nuke/issues/844
	public static GitCompareReport GitGetCompareReport(string localGitFolder, string? oldBranchName = null)
	{
		localGitFolder = localGitFolder.Replace("\\", "/");
		if (oldBranchName == null)
		{
			if (TryGetBranchToCompareName(out oldBranchName) is false)
			{
				return new GitCompareReport(localGitFolder, false, Array.Empty<SolutionChangeReport>());
			}
		}

		using (var repo = new Repository(localGitFolder))
		{
			GetBranchesToCompare(repo, oldBranchName, out var newBranchLocal, out var oldBranchLocal);
			Log.Information($"Creating change report between '{newBranchLocal.FriendlyName}:{newBranchLocal.Tip.Id.ToString().Substring(0, 6)}:{newBranchLocal.Tip.MessageShort}' -> '{newBranchLocal.FriendlyName}:{oldBranchLocal.Tip.Id.ToString().Substring(0, 6)}:{oldBranchLocal.Tip.MessageShort}'");
			List<(string solutionPath, bool solutionChanged, List<string> solutionItems, List<(string projectPath, bool projectChanged, List<string> fileChanges)> projectChanges)> solutionChanges = new();

			var paths = repo.Diff.Compare<TreeChanges>(oldBranchLocal.Tip.Tree, newBranchLocal.Tip.Tree)
					.Where(x => x.Exists)
					.Select(x => x.Path);
			SortedSet<string> changesGitRelativePaths = new(paths);

			if (repo.HasUncommitedChanges())
			{
				repo.GetUncommitedRemovedChanges()
					.ForEach(x => changesGitRelativePaths.Remove(x));
				repo.GetUncommitedChanges()
					.ForEach(x => changesGitRelativePaths.Add(x));
			}

			string? projectDirectoryRelativePath = null;
			bool projectAlreadyFound = false;
			string? solutionDirectoryRelativePath = null;
			bool solutionAlreadyFound = false;
			string? lastCheckedDirectoryGitRelativePath = null;

			foreach (string? changeRelativePath in changesGitRelativePaths)
			{
				string changeFullPath = $"{localGitFolder}/{changeRelativePath}";

				if (changeRelativePath.EndsWith(solutionExtension))
				{
					bool isChangeInGitRoot = changeRelativePath.IndexOf("/") == -1;
					bool solutionIsInGitRoot = solutionDirectoryRelativePath == gitRoot && isChangeInGitRoot;
					string changeParentDir = GetGitParentDirectoryRelativePath(changeRelativePath);
					if (solutionIsInGitRoot || solutionDirectoryRelativePath == changeParentDir)
					{
						var sol = solutionChanges.Last();
						sol.solutionChanged = true;
						continue;
					}

					Serilog.Log.Debug($"Adding solution because: Solution found. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{changeRelativePath}'. Full change path: '{changeFullPath}'");
					solutionChanges.Add((changeFullPath, true, new(), new()));
					solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(changeRelativePath);
					solutionAlreadyFound = true;
					continue;
				}

				if (changeRelativePath.EndsWith(projectExtension))
				{
					if (projectDirectoryRelativePath == GetGitParentDirectoryRelativePath(changeRelativePath))
					{
						var (projectPath, projectChanged, fileChanges) = solutionChanges.Last().projectChanges.Last();
						projectChanged = true;
						continue;
					}

					string solutionFilePath = GetSolution(changeFullPath);

					string solRelativePath = GetGitRelativePath(solutionFilePath!, localGitFolder);
					bool solutionIsInGitRootSameAsLastOne = solutionDirectoryRelativePath == gitRoot && solRelativePath.IndexOf("/") == -1;
					if (!((solutionAlreadyFound && solutionIsInGitRootSameAsLastOne) || GetGitParentDirectoryRelativePath(solRelativePath) == solutionDirectoryRelativePath))
					{
						Log.Debug($"Adding solution because: Project found. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{changeRelativePath}'. Full change path: '{changeFullPath}'");
						solutionChanges.Add((solutionFilePath!, false, new(), new()));
						solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(solutionFilePath!, localGitFolder));
						solutionAlreadyFound = true;
					}

					solutionChanges.Last().projectChanges.Add((changeFullPath, true, new()));
					projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(changeRelativePath);
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
							projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(projectFullPath2, localGitFolder));
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

					if (changeRelativePath.StartsWith(projectDirectoryRelativePath!))
					{
						solutionChanges.Last().projectChanges.Last().fileChanges.Add(changeFullPath);
						continue;
					}
					else
					{
						lastCheckedDirectoryGitRelativePath = null;
					}
				}

				string solutionFilePath2 = GetSolution(changeFullPath);

				if (!solutionChanges.Any() || solutionChanges.Last().solutionPath != solutionFilePath2)
				{
					Serilog.Log.Debug($"Adding solution because: Nothing cached and last solution does not match. Old solution relative path: '{solutionDirectoryRelativePath}'. Change path: '{changeRelativePath}'. Full change path: '{changeFullPath}'");
					solutionChanges.Add((solutionFilePath2!, false, new(), new()));
					solutionDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(solutionFilePath2!, localGitFolder));
					solutionAlreadyFound = true;
				}

				if (TryGetProject(changeFullPath, out string? projectFullPath))
				{
					solutionChanges.Last().projectChanges.Add((projectFullPath, false, new()));
					solutionChanges.Last().projectChanges.Last().fileChanges.Add(changeFullPath);
					projectDirectoryRelativePath = GetGitParentDirectoryRelativePath(GetGitRelativePath(projectFullPath, localGitFolder));
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
					solutionChanges => new SolutionChangeReport(solutionChanges.solutionPath, solutionChanges.solutionChanged, solutionChanges.solutionItems
						.Select(filePath => new FileChange(filePath))
						.ToArray(), solutionChanges.projectChanges
						.Select(projectChanges => new ProjectChangeReport(projectChanges.projectPath, projectChanges.projectChanged, projectChanges.fileChanges
							.Select(filePath => new FileChange(filePath))
							.ToArray()))
						.ToArray()))
				.ToArray();

			var report = new GitCompareReport(localGitFolder, true, projectChanges);
			LogReport(report);
			return report;
		}
	}

	private static void LogReport(GitCompareReport report)
	{
		Log.Information("Added or modified files:");
		foreach (var solution in report.ChangedSolutions)
		{
			Log.Information($"  Solution: {solution.SolutionFullPath}");
			foreach (var solutionItem in solution.SolutionItemsChanges)
			{
				Log.Information($"    Solution item: {solutionItem.FullPath}");
			}

			foreach (var project in solution.ChangedProjects)
			{
				Log.Information($"    Project: {project.ProjectFullPath}");
				foreach (var file in project.FileChanges)
				{
					Log.Information($"      File: {file.FullPath}");
				}
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
		string log = "";
		Commands.Fetch(repo, remoteSource.Name, new[] { $"{oldBranchName}:{oldBranchName}" }, new FetchOptions(), log);
		oldBranchLocal = repo.Branches[oldBranchName];
	}

	private static string GetGitParentDirectoryRelativePath(string gitRelativePath)
	{
		string? directoryToSkip;
		var gitRelativePathSpan = gitRelativePath.AsSpan();
		int lastPathSeparator = gitRelativePathSpan.LastIndexOf('/');
		if (lastPathSeparator is -1)
		{
			return gitRoot;
		}

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
			fileDirectory = new DirectoryInfo(file.Directory!.FullName);

		}
		else
		{
			fileDirectory = new DirectoryInfo(fullPath);
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
		else
		{
			projectPath = csprojFile.FullName.Replace('\\', '/');
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
				throw new ArgumentException($"Path '{path}' does not exists or is not valid file or directory path", nameof(path));
			}
		}
	}

	private static string GetSolution(string fullPath)
	{
		if (TryGetSolution(fullPath, out string? solutionFullPath) is false)
		{
			throw new Exception($"Soltion for file '{fullPath}' not found");
		}

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
			fileDirectory = new DirectoryInfo(file.Directory!.FullName);

		}
		else
		{
			fileDirectory = new DirectoryInfo(fullPath);
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
		else
		{
			solutionFullPath = solutionFile.FullName.Replace('\\', '/');
			return true;
		}
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
