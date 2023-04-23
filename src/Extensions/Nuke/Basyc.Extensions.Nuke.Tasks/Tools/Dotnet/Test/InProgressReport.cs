using Basyc.Extensions.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public class InProgressReport
{
    private readonly Dictionary<string, InProgressProjectReport> allReports = new();

    public int ProjectCount { get; private set; }

    public InProgressProjectReport GetReport(string projectName) => allReports[projectName];

    public void Complete(string projectName, ProjectCoverageReport report) => GetReport(projectName).Report = report;

    public void Add(string projectToTestName, string? testProjectPath, bool testProjectFound, bool isExcluded)
    {
        allReports.Add(projectToTestName, new(projectToTestName, testProjectPath, testProjectFound, isExcluded));
        ProjectCount += 1;
        if (testProjectFound is false)
        {
            Complete(projectToTestName, new(projectToTestName, testProjectFound, isExcluded, 0, 0, Array.Empty<ClassCoverageReport>()));
            return;
        }

        if (isExcluded)
        {
            Complete(projectToTestName, new(projectToTestName, testProjectFound, isExcluded, 0, 0, Array.Empty<ClassCoverageReport>()));
        }
    }

#pragma warning disable SA1316 // Tuple element names should use correct casing
    public void AddRange(
        IEnumerable<(string projectToTestName, string? testProjectPath, bool testProjectFound, bool shouldBeExcluded)> items, UnitTestSettings testExceptions) => items.ForEach(x =>
    {
        Add(x.projectToTestName, x.testProjectPath, x.testProjectFound, x.shouldBeExcluded);
    });
#pragma warning restore SA1316 // Tuple element names should use correct casing

    public void AddRange(Solution solution, IEnumerable<string> sourceProjectsPaths, string testProjectSuffix, UnitTestSettings testExceptions) => AddRange(
        sourceProjectsPaths.Select(projectToTestPath =>
        {
            var projectToTest = solution.GetProject(projectToTestPath.NormalizeForCurrentOs());
            var projectToTestAttributes = projectToTest.GetItems("AssemblyAttribute");
            bool excluded = projectToTestAttributes.Contains("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") ||
                            testExceptions.ProjectExceptions.Any(y => y.Path == projectToTestPath);

            string projectName = Path.GetFileNameWithoutExtension(projectToTestPath);
            string unitTestProjectName = Path.GetFileNameWithoutExtension(projectToTestPath) + testProjectSuffix;
            var testProject = solution!.GetProject(unitTestProjectName);
            if (testProject is null)
            {
                return (projectName, null!, false, excluded);
            }

            return (projectName, testProject.Path.ToString(), true, excluded);
        })!,
        testExceptions);

    public void AddSolution(Solution solution, string testProjectSuffix, UnitTestSettings testExceptions)
    {
        string[] sourceProjectsPaths = solution.AllProjects
            .Where(x => x.Name.EndsWith(testProjectSuffix) is false)
            .Select(x => x.Path.ToString())
            .ToArray();

        AddRange(sourceProjectsPaths.Select(projectToTestPath =>
            {
                var projectToTest = solution.GetProject(projectToTestPath);
                var projectToTestAttributes = projectToTest.GetItems("AssemblyAttribute");
                bool excluded = projectToTestAttributes.Contains("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute");

                string projectName = Path.GetFileNameWithoutExtension(projectToTestPath);
                string unitTestProjectName = Path.GetFileNameWithoutExtension(projectToTestPath) + testProjectSuffix;
                var testProject = solution!.GetProject(unitTestProjectName);
                if (testProject is null)
                {
                    return (projectName, null!, false, excluded);
                }

                return (projectName, testProject.Path.ToString(), true, excluded);
            })!,
            testExceptions);
    }

    public InProgressProjectReport[] GetAllReports() => allReports.Values.ToArray();

    public InProgressProjectReport[] GetReportsToExecute()
    {
        var testProjectsToRun = GetAllReports()
            .Where(x => x.TestProjectFound && x.CoverageExcluded is false)
            .ToArray();

        return testProjectsToRun;
    }
}
