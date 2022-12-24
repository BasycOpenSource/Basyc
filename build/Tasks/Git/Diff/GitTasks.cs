using GlobExpressions;
using LibGit2Sharp;
using System.Diagnostics.CodeAnalysis;
using Tasks.Git.Diff;

namespace _build;

public static partial class GitTasks
{
	private const string gitRoot = ".\\";

	public static void GitDiff()
	{

	}

	//ProjectModelTasks.Initialize(); //https://github.com/nuke-build/nuke/issues/844

	public static GitCompareReport GitGetCompareReport(string localGitFolder, string? branchToCompare = null)
	{
		bool couldCompare = branchToCompare != null;
		if (couldCompare is false)
		{
			return new GitCompareReport(localGitFolder, false, Array.Empty<SolutionChangeReport>());
		}

		string newBranchName = Nuke.Common.Tools.Git.GitTasks.GitCurrentBranch();
		string newBranchCommintId = Nuke.Common.Tools.Git.GitTasks.GitCurrentCommit();

		using (var repo = new Repository(localGitFolder))
		{
			//TODO: Include uncommited changes
			var oldBranch = repo.Branches[branchToCompare];
			var newBranch = repo.Branches[newBranchName];
			var newBranchCommit = newBranch.Commits.First(x => x.Id.ToString() == newBranchCommintId);

			Serilog.Log.Information($"Creating change report between '{newBranchName}:{newBranchCommit.Id.ToString().Substring(0, 6)}:{newBranchCommit.MessageShort}' -> '{branchToCompare}:{oldBranch.Tip.Id.ToString().Substring(0, 6)}:{oldBranch.Tip.MessageShort}'");

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
					bool isChangeInGitRoot = change.Path.IndexOf("/") == -1;
					bool solutionIsInGitRoot = solutionDirectoryRelativePath == gitRoot && isChangeInGitRoot;
					string changeParentDir = GetGitParentDirectoryRelativePath(change.Path);
					if (solutionIsInGitRoot || solutionDirectoryRelativePath == changeParentDir)
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
					bool solutionIsInGitRootSameAsLastOne = solutionDirectoryRelativePath == gitRoot && solRelativePath.IndexOf("/") == -1;
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
					solutionChanges => new SolutionChangeReport(solutionChanges.solutionPath, solutionChanges.solutionChanged, solutionChanges.solutionItems
						.Select(filePath => new FileChange(filePath))
						.ToArray(), solutionChanges.projectChanges
						.Select(projectChanges => new ProjectChangeReport(projectChanges.projectPath, projectChanges.projectChanged, projectChanges.fileChanges
							.Select(filePath => new FileChange(filePath))
							.ToArray()))
						.ToArray()))
				.ToArray();
			return new(localGitFolder, couldCompare, projectChanges);
		}
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
		var gitRelativePath = filePathSpan.Slice(gitRoot.Length + 1);
		return gitRelativePath.ToString();
	}
}