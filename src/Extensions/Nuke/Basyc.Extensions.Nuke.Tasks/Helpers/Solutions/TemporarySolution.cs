using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using System.Diagnostics.CodeAnalysis;
using static Nuke.Common.ProjectModel.ProjectModelTasks;

namespace Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;

[ExcludeFromCodeCoverage]
public readonly struct TemporarySolution : IDisposable
{
    public TemporarySolution(Solution solution)
    {
        Solution = solution;
    }

    public Solution Solution { get; }

    /// <summary>
    ///     Creates new <see cref="TemporarySolution" /> containing all projects from existing solution excluding build project.
    /// </summary>
    public static TemporarySolution CreateNew(Solution solution, string buildProjectName)
    {
        string temporarySolutionFilePath = TemporaryFile.GetNew("temporary.generated", "sln", solution.Directory);
        var temporarySolution = CreateSolution($"{temporarySolutionFilePath}", new[] { solution }, x => x == solution ? null : x.Name);
        TryRemoveBuildProject(buildProjectName, temporarySolution);

        temporarySolution.Save();
        return new(temporarySolution);
    }

    /// <summary>
    ///     Creates new <see cref="TemporarySolution" /> containing specified projecs from existing solution exluding build project.
    /// </summary>
    public static TemporarySolution CreateNew(Solution solution, string? buildProjectName, IEnumerable<string> projectsPaths)
    {
        string temporarySolutionFilePath = TemporaryFile.GetNew("temporary.generated", "sln", solution.Directory);
        var temporarySolution = CreateSolution($"{temporarySolutionFilePath}", new[] { solution }, x => x == solution ? null : x.Name);

        var projectsPathsSet = projectsPaths.ToHashSet();
        temporarySolution.AllProjects
            .Where(x => projectsPathsSet.Contains(x.Path.ToString().NormalizePath()) is false)
            .ForEach(temporarySolution.RemoveProject);
        TryRemoveBuildProject(buildProjectName, temporarySolution);

        temporarySolution.Save();
        return new(temporarySolution);
    }

    public static TemporarySolution GetAffectedAsSolution(
    RepositoryChangeReport gitCompareReport,
    string unitTestSuffix,
    string buildProjectName,
    Solution solution)
    {
        var changedProjectsPaths = gitCompareReport.ChangedSolutions
            .SelectMany(x => x.ChangedProjects)
            .Select(x => x.ProjectFullPath);

        changedProjectsPaths = changedProjectsPaths.Concat(changedProjectsPaths.Select(x =>
        {
            var testProject = solution.GetProject(Path.GetFileNameWithoutExtension(x) + unitTestSuffix);
            return testProject is null ? null! : testProject.Path.ToString().NormalizePath();
        }).Where(x => x is not null));
        var solutionToUse = CreateNew(solution, buildProjectName, changedProjectsPaths);
        return solutionToUse;
    }

    public void Dispose() => File.Delete(Solution);

    private static void TryRemoveBuildProject(string? buildProjectName, Solution temporarySolution)
    {
        if (buildProjectName is not null)
        {
            var buildProject = temporarySolution.GetProject(buildProjectName);
            if (buildProject is not null)
                temporarySolution.RemoveProject(buildProject);
        }
    }
}
