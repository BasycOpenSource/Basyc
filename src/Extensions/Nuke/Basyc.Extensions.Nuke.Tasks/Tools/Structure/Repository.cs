using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Nuke.Common.IO;
using System.Diagnostics.CodeAnalysis;
using static Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.DotNetTasks;


namespace Basyc.Extensions.Nuke.Tasks.Tools.Structure;

public class Repository
{
	public Repository(string rootDirectory)
	{
		DirectoryPath = (AbsolutePath)rootDirectory;
		Documents = new Documents(DirectoryPath);
		Source = new Source(DirectoryPath);
		Tests = new Tests(DirectoryPath);
		TestsHistory = new TestsHistory(DirectoryPath);
	}

	public AbsolutePath DirectoryPath { get; }
	public Documents Documents { get; }
	public Source Source { get; }
	public Tests Tests { get; }
	public TestsHistory TestsHistory { get; }
}

public record Documents(string RootDirectory)
{
	public AbsolutePath DirectoryPath => RepositoryStructureHelper.GetDocsFolder(RootDirectory);
}

public record Source(string RootDirectory)
{
	public AbsolutePath DirectoryPath => RepositoryStructureHelper.GetSourceFolder(RootDirectory);
}

public record Tests(string RootDirectory)
{
	public AbsolutePath DirectoryPath => RepositoryStructureHelper.GetTestsFolder(RootDirectory);
}

public record TestsHistory(string RootDirectory)
{
	public AbsolutePath DirectoryPath => RepositoryStructureHelper.GetTestsHistoryFolder(RootDirectory);

	/// <returns>File path to new history file</returns>
	public string AddOrUpdateHistory(string branchName, CoverageReport coverageReport)
	{
		var newCoverageFilePath = GetHistoryFile(branchName);
		BasycCoverageSaveToFile(coverageReport, newCoverageFilePath);
		return newCoverageFilePath;
	}

	public bool TryGetHistory(string branchName, [NotNullWhen(true)] out CoverageReport? report)
	{
		var historyFile = $"{DirectoryPath / NormalizeBranchNameToFileSystem(branchName)}.json";
		if (File.Exists(historyFile) is false)
		{
			report = null;
			return false;
		}

		report = BasycCoverageLoadFromFile(historyFile);
		return true;
	}

	private string GetHistoryFile(string branchName)
	{
		return $"{DirectoryPath / NormalizeBranchNameToFileSystem(branchName)}.json";
	}

	public void DeleteHistory(string branchName)
	{
		var branchHistoryFile = GetHistoryFile(branchName);
		File.Delete(branchHistoryFile);
	}

	private static string NormalizeBranchNameToFileSystem(string branchName)
	{
		return branchName.Replace('/', '-');
	}
}
