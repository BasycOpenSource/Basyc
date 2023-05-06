﻿using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.File;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using File = System.IO.File;

// ReSharper disable CheckNamespace
namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet;

public static partial class DotNetTasks
{
    private static readonly XmlSerializer xmlSerializer = new(typeof(CoverageSession));

    public static CoverageReport BasycUnitTestAffected(
        Solution solution,
        RepositoryChangeReport gitCompareReport,
        string testProjectSuffix,
        UnitTestSettings projectTestException)
    {
        var projectsToTestPaths = gitCompareReport.ChangedSolutions
            .SelectMany(x => x.ChangedProjects)
            .Select(x => x.ProjectFullPath)
            .Where(x => Path.GetFileNameWithoutExtension(x).EndsWith(testProjectSuffix) is false)
            .ToHashSet();

        string[] changedTestProjectPaths = gitCompareReport.ChangedSolutions
            .SelectMany(x => x.ChangedProjects)
            .Select(x => x.ProjectFullPath)
            .Where(x => Path.GetFileNameWithoutExtension(x).EndsWith(testProjectSuffix))
            .ToArray();

        if (changedTestProjectPaths.Any())
        {
            foreach (string changedTestProjectPath in changedTestProjectPaths)
            {
                string projectName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(changedTestProjectPath));
                string projectPath = solution.GetProject(projectName).Path.ToString().NormalizePath();
                projectsToTestPaths.Add(projectPath);
            }
        }

        return UnitTest(solution, projectsToTestPaths, testProjectSuffix, projectTestException);
    }

    public static CoverageReport BasycUnitTestAll(Solution solution, string testProjectSuffix, UnitTestSettings projectTestException)
    {
        var sourceProjects = solution.AllProjects
            .Where(x => x.Name.EndsWith(testProjectSuffix) is false)
            .Select(x => x.Path.ToString());
        return UnitTest(solution, sourceProjects, testProjectSuffix, projectTestException);
    }

    public static void BasycTestAssertMinimum(CoverageReport report, double minSequenceCoverage, double minBranchCoverage)
    {
        const string indent = "  ";
        List<string> errors = new();
        var classErrorStringBuilder = new StringBuilder();
        report.Projects.ForEach(projectReport =>
        {
            if (projectReport.CoverageExcluded)
                return;

            if (projectReport.SequenceCoverage >= minSequenceCoverage && projectReport.BranchCoverage >= minBranchCoverage)
                return;

            classErrorStringBuilder.AppendLine($"Project '{projectReport.Name}'");
            projectReport.Classes.ForEach(classReport =>
            {
                var classErrors = new List<string>();
                if (classReport.SequenceCoverage < minSequenceCoverage)
                    classErrors.Add($"Sequence coverage {classReport.SequenceCoverage}% should be {minSequenceCoverage}%");

                if (classReport.BranchCoverage < minBranchCoverage)
                    classErrors.Add($"Branch coverage {classReport.BranchCoverage}% should be {minBranchCoverage}%");

                if (classErrors.Any())
                {
                    classErrorStringBuilder.Append(indent);
                    classErrorStringBuilder.Append($"Class '{classReport.Name}' ");
                    classErrors.ForEach(classError =>
                    {
                        classErrorStringBuilder.Append(classError);
                        classErrorStringBuilder.Append(' ');
                    });
                }

                if (classReport.Methods.Any())
                    classErrorStringBuilder.AppendLine();

                classReport.Methods.ForEach(method =>
                {
                    var methodErrors = new List<string>();

                    if (method.SequenceCoverage < minSequenceCoverage)
                        methodErrors.Add($"Sequence coverage {method.SequenceCoverage}% should be {minSequenceCoverage}%.");

                    if (method.BranchCoverage < minBranchCoverage)
                        methodErrors.Add($"Branch coverage {method.BranchCoverage}% should be {minBranchCoverage}%.");

                    if (methodErrors.Any())
                    {
                        classErrorStringBuilder.Append(indent);
                        classErrorStringBuilder.Append(indent);
                        int methodNameIndex = method.Name.IndexOf("::") + 2;
                        string shortMethodName = method.Name.Substring(methodNameIndex);
                        classErrorStringBuilder.Append($"Method '{shortMethodName}' ");
                        methodErrors.ForEach(methodError =>
                        {
                            classErrorStringBuilder.Append(methodError);
                            classErrorStringBuilder.Append(' ');
                        });
                    }

                    classErrorStringBuilder.AppendLine();
                });
                classErrorStringBuilder.AppendLine();
            });
            errors.Add(classErrorStringBuilder.ToString());
            classErrorStringBuilder.Clear();
        });

        if (errors.Any())
            throw new InvalidOperationException("Some projects do not meet coverage minimum.");
    }

    public static void BasycCoverageSaveToFile(CoverageReport coverageReport, string path)
    {
        var dto = CoverageReportJsonDto.ToDto(coverageReport);
        string json = JsonSerializer.Serialize(dto);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        if (File.Exists(path))
            File.Delete(path);
        File.CreateText(path).Dispose();
        File.WriteAllText(path, json);
    }

    public static CoverageReport BasycCoverageLoadFromFile(string path)
    {
        string json = File.ReadAllText(path);
        var dto = JsonSerializer.Deserialize<CoverageReportJsonDto>(json)!;
        var report = CoverageReportJsonDto.ToReport(dto);
        return report;
    }

    private static CoverageReport UnitTest(
        Solution solution,
        IEnumerable<string> projectToTestPaths,
        string testProjectSuffix,
        UnitTestSettings testExceptions)
    {
        var inProgressReport = new InProgressReport();
        inProgressReport.AddRange(solution, projectToTestPaths, testProjectSuffix, testExceptions);

        var projectReports = inProgressReport.GetReportsToExecute();
        using var testRunSettings = new TestRunSettingsMultiple(projectReports.Select(x => x.ProjectToTestName));

        var projectResultDirectory = TemporaryDirectory.CreateNew($"{nameof(UnitTest)}/Projects");
        DotNetTest(_ => _
                .EnableNoRestore()
                .EnableNoBuild()
                .EnableCollectCoverage()
                .CombineWith(projectReports, (settings, projectReport) => settings
                    .SetSettingsFile(testRunSettings.GetForProject(projectReport.ProjectToTestName).FullPath)
                    .SetResultsDirectory(projectResultDirectory.FullPath + "/" + projectReport.ProjectToTestName)
                    .SetProjectFile(projectReport.TestProjectPath)),
            5);

        foreach (var testProjectReport in projectReports)
        {
            var dir = new DirectoryInfo(projectResultDirectory.FullPath + "/" + testProjectReport.ProjectToTestName);
            var uniqueNameDir = dir.GetDirectories().First();

            string openCoverResultsFilePath = Path.Combine(uniqueNameDir.FullName, "coverage.opencover.xml");
            using var outputFileStream = File.OpenRead(openCoverResultsFilePath);
#pragma warning disable CA5369 // Use XmlReader for 'XmlSerializer.Deserialize()'
            var openCoverCoverageSession = (CoverageSession)xmlSerializer.Deserialize(outputFileStream)!;
#pragma warning restore CA5369 // Use XmlReader for 'XmlSerializer.Deserialize()'
            var openCoverReport = openCoverCoverageSession.Modules.Value().Module.Value()
                .Select(ParseOpencoverModule)
                .FirstOrDefault();

            if (openCoverReport == default)
            {
                //When parsing a open cover file returns 0 modules.
                //It means that there are 0 tests inside the test project
                //that is testing project to test.
                inProgressReport.Complete(testProjectReport.ProjectToTestName,
                    new(testProjectReport.ProjectToTestName, true, false, 0, 0, Array.Empty<ClassCoverageReport>()));
            }
            else
            {
                inProgressReport.Complete(testProjectReport.ProjectToTestName, openCoverReport);
            }
        }

        //for all bacthes etc. :
        //dotnet test /p:CollectCoverage=true /p:MergeWith='/path/to/result.json'
        var projectCoverageReports = inProgressReport.GetAllReports().Select(x => x.Report!).ToArray();
        return new(projectResultDirectory, projectCoverageReports);
    }

    private static void LogTestReport(InProgressReport inProgressReport)
    {
        Log.Debug("Coverage report:");
        var projectReports = inProgressReport.GetAllReports();
        foreach (var projectReport in projectReports)
        {
            Log.Debug(
                $"		Assembly: {projectReport.ProjectToTestName} BranchCoverage: {projectReport!.Report!.BranchCoverage}% SequenceCoverage: {projectReport.Report.SequenceCoverage}% TestFound: {projectReport!.Report!.TestProjectFound} Excluded: {projectReport!.Report.CoverageExcluded}");

            foreach (var classReport in projectReport.Report.Classes)
            {
                Log.Debug($"			Class: {classReport.Name} BranchCoverage: {classReport.BranchCoverage}% SequenceCoverage: {classReport.SequenceCoverage}%");
                foreach (var methodReport in classReport.Methods)
                {
                    Log.Debug(
                        $"				Method: {classReport.Name} BranchCoverage: {methodReport.BranchCoverage}% SequenceCoverage: {methodReport.SequenceCoverage}%");
                }
            }
        }
    }

    private static string GetTestedProjectName(string testProjectPath, string unitTestProjectNameSuffix)
    {
        var sourceProjectName = Path.GetFileNameWithoutExtension(testProjectPath).AsSpan()
            .Slice(0, testProjectPath.Length - unitTestProjectNameSuffix.Length);
        return $"{sourceProjectName}";
    }

    private static bool TryGetTestProjectPath(string projectToTestPath, Solution solution, string testProjectSuffix, out string? testProjectPath)
    {
        string unitTestProjectName = Path.GetFileNameWithoutExtension(projectToTestPath) + testProjectSuffix;
        var unitTestProject = solution!.GetProject(unitTestProjectName);
        if (unitTestProject is null)
        {
            testProjectPath = null;
            return false;
        }

        testProjectPath = unitTestProject.Path.ToString().Replace('\\', '/');
        return true;
    }

    private static ProjectCoverageReport ParseOpencoverModule(Module module)
    {
        double projectBranchCoverage = Math.Round(module.Classes.Value().Class.Value()
            .Select(x => double.Parse(x.Summary.Value().BranchCoverage.Value(), CultureInfo.InvariantCulture.NumberFormat))
            .Average());
        double projectSequenceCoverage = Math.Round(module.Classes.Value().Class.Value()
            .Select(x => double.Parse(x.Summary.Value().SequenceCoverage.Value(), CultureInfo.InvariantCulture.NumberFormat))
            .Average());

        return new(
            module.ModuleName.Value(),
            true,
            false,
            projectBranchCoverage,
            projectSequenceCoverage,
            module.Classes.Value().Class.Value()
                .Select(classDto =>
                {
                    string className = Path.GetFileNameWithoutExtension(classDto.FullName + ".cs");
                    double branchCoverage = ParseDouble(classDto.Summary.Value().BranchCoverage.Value());
                    double sequenceCoverage = ParseDouble(classDto.Summary.Value().SequenceCoverage.Value());

                    return new ClassCoverageReport(className, branchCoverage, sequenceCoverage, classDto.Methods.Value().Method.Value()
                        .Select(methodDto =>
                            new MethodCoverageReport(methodDto.Name.Value(),
                                ParseDouble(methodDto.BranchCoverage.Value()),
                                ParseDouble(methodDto.SequenceCoverage.Value())))
                        .ToArray());
                })
                .ToArray());
    }

    private static double ParseDouble(string number) => double.Parse(number, CultureInfo.InvariantCulture.NumberFormat);
}
