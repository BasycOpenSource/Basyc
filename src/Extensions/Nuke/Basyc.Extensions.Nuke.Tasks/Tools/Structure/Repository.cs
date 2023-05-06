using System.Diagnostics.CodeAnalysis;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Nuke.Common.IO;
using static Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Structure;

public class Repository
{
    public Repository(string rootDirectory)
    {
        DirectoryPath = (AbsolutePath)rootDirectory;
        Documents = new(DirectoryPath);
        Source = new(DirectoryPath);
        Tests = new(DirectoryPath);
        TestsHistory = new(DirectoryPath);
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

    /// <summary> Returns file path to new history file.</summary>
    public string AddOrUpdateHistory(string branchName, CoverageReport coverageReport)
    {
        string newCoverageFilePath = GetHistoryFile(branchName);
        BasycCoverageSaveToFile(coverageReport, newCoverageFilePath);
        return newCoverageFilePath;
    }

    public bool TryGetHistory(string branchName, [NotNullWhen(true)] out CoverageReport? report)
    {
        string historyFile = $"{DirectoryPath / NormalizeBranchNameToFileSystem(branchName)}.json";
        if (File.Exists(historyFile) is false)
        {
            report = null;
            return false;
        }

        report = BasycCoverageLoadFromFile(historyFile);
        return true;
    }

    private string GetHistoryFile(string branchName) => $"{DirectoryPath / NormalizeBranchNameToFileSystem(branchName)}.json";

    public void DeleteHistory(string branchName)
    {
        string branchHistoryFile = GetHistoryFile(branchName);
        File.Delete(branchHistoryFile);
    }

    private static string NormalizeBranchNameToFileSystem(string branchName) => branchName.Replace('/', '-');
}
