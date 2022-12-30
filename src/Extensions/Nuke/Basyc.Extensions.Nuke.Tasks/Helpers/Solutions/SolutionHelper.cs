namespace Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
public static class SolutionHelper
{

    /// <summary>
    /// Creates new <see cref="TemporarySolution"/> containing all projects from existing solution exluding build project
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="buildProjectName"></param>
    /// <returns></returns>
    public static TemporarySolution NewTempSolution(Solution solution, string buildProjectName)
    {
        string uniqueName = TemporaryFile.GetNewName("temporary.generated", "sln");
        var newSolution = CreateSolution($"{uniqueName}", new[] { solution }, folderNameProvider: x => x == solution ? null : x.Name);
        newSolution.RemoveProject(newSolution.GetProject(buildProjectName));
        newSolution.Save();
        return new TemporarySolution(newSolution);
    }

    /// <summary>
    /// Creates new <see cref="TemporarySolution"/> containing specified projecs from existing solution exluding build project
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="buildProjectName"></param>
    /// <param name="projectsPaths"></param>
    /// <returns></returns>
    public static TemporarySolution NewTempSolution(Solution solution, string? buildProjectName, IEnumerable<string> projectsPaths)
    {
        var projectsPathsSet = projectsPaths.ToHashSet();
        string uniqueName = TemporaryFile.GetNewName("temporary.generated", "sln");

        var newSolution = CreateSolution($"{uniqueName}", new[] { solution }, folderNameProvider: x => x == solution ? null : x.Name);

        newSolution.AllProjects
            .Where(x => projectsPathsSet.Contains(x.Path.ToString().NormalizePath()) is false)
            .ForEach(newSolution.RemoveProject);
        if (buildProjectName is not null)
        {
            var buildProject = newSolution.GetProject(buildProjectName);
            if (buildProject is not null)
            {
                newSolution.RemoveProject(buildProject);
            }
        }

        newSolution.Save();
        return new TemporarySolution(newSolution);
    }

    public static TemporarySolution GetAffectedAsSolution(GitCompareReport gitCompareReport, string unitTestSuffix, string buildProjectName, Solution solution)
    {
        var changedProjectsPaths = gitCompareReport.ChangedSolutions
            .SelectMany(x => x.ChangedProjects)
            .Select(x => x.ProjectFullPath);

        changedProjectsPaths = changedProjectsPaths.Concat(changedProjectsPaths.Select(x =>
        {
            var testProject = solution.GetProject(Path.GetFileNameWithoutExtension(x) + unitTestSuffix);
            if (testProject is null)
            {
                return null!;
            }

            return testProject.Path.ToString().NormalizePath();
        }).Where(x => x is not null));
        var solutionToUse = SolutionHelper.NewTempSolution(solution, buildProjectName, changedProjectsPaths);
        return solutionToUse;
    }
}
