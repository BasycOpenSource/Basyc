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
        string uniqueName = TemporaryFile.GetNew("temporary.generated", "sln");
        var newSolution = CreateSolution($"{uniqueName}", new[] { solution }, x => x == solution ? null : x.Name);
        newSolution.RemoveProject(newSolution.GetProject(buildProjectName));
        newSolution.Save();
        return new(newSolution);
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

    /// <summary>
    ///     Creates new <see cref="TemporarySolution" /> containing specified projecs from existing solution exluding build project.
    /// </summary>
    public static TemporarySolution CreateNew(Solution solution, string? buildProjectName, IEnumerable<string> projectsPaths)
    {
        var projectsPathsSet = projectsPaths.ToHashSet();
        string temporarySolutionFilePath = TemporaryFile.GetNew("temporary.generated", "sln", solution.Directory);

        var newSolution = CreateSolution($"{temporarySolutionFilePath}", new[] { solution }, x => x == solution ? null : x.Name);

        newSolution.AllProjects
            .Where(x => projectsPathsSet.Contains(x.Path.ToString().NormalizePath()) is false)
            .ForEach(newSolution.RemoveProject);
        if (buildProjectName is not null)
        {
            var buildProject = newSolution.GetProject(buildProjectName);
            if (buildProject is not null)
                newSolution.RemoveProject(buildProject);
        }

        newSolution.Save();
        return new(newSolution);
    }

    public void Dispose() => File.Delete(Solution);
}
