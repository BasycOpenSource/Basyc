using Nuke.Common.ProjectModel;
using Serilog;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;

public record RepositoryChangeReport(string GitRepoLocalDirectory, bool CouldCompare, SolutionChangeReport[] ChangedSolutions)
{
    public HashSet<string> GetTestProjectsToRun(Solution solution, string testProjectNameSuffix)
    {
        string testProjectFileNameEnding = $"{testProjectNameSuffix}.csproj";

        var unitTestProjectsPaths = ChangedSolutions
            .SelectMany(x => x.ChangedProjects)
            .Select(x => x.ProjectFullPath)
            .Where(x => x.EndsWith(testProjectFileNameEnding))
            .ToHashSet();

        var changedSourceProjectsPaths = ChangedSolutions
            .SelectMany(x => x.ChangedProjects)
            .Select(x => x.ProjectFullPath)
            .Where(x => x.EndsWith(testProjectFileNameEnding) is false);

        foreach (string? changedProject in changedSourceProjectsPaths)
        {
            string unitTestProjectName = Path.GetFileNameWithoutExtension(changedProject) + testProjectNameSuffix;
            var unitTestProject = solution!.GetProject(unitTestProjectName);
            if (unitTestProject is null)
            {
                Log.Warning($"Unit test project with name '{unitTestProjectName}' for '{changedProject}' not found.");
                continue;
            }

            unitTestProjectsPaths.Add(unitTestProject.Path.ToString().Replace('\\', '/'));
        }

        return unitTestProjectsPaths;
    }
}
