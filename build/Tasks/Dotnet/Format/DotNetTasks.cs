using NuGet.Packaging;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Tasks.Dotnet;
using Tasks.Dotnet.Format;
using Tasks.Git.Diff;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
namespace _build;

public static partial class DotNetTasks
{
    private static bool DotnetFormatVerifyNoChanges(string workingDirectory, string projectOrSolutionPath, IEnumerable<string> filesTocheck, [NotNullWhen(false)] out AggregatedDotnetFormatReport? aggregatedReport, out ProcessException? processException)
    {
        var isFormated = DotnetWrapper.FormatVerifyNoChanges(workingDirectory, projectOrSolutionPath, filesTocheck, out var report, out processException);
        if (isFormated)
        {
            if (filesTocheck is null || filesTocheck.Any() is false)
            {
                Log.Information($"Project '{projectOrSolutionPath}' is formatted.");
            }
            else
            {
                var filesToCheckString = string.Join("\n", filesTocheck);
                Log.Information($"These files or directories were checked are formatted correctly:\n{filesToCheckString}");
            }
            aggregatedReport = null;
            return true;
        }
        var isSolution = projectOrSolutionPath.EndsWith(".sln");

        if (isSolution)
        {
            aggregatedReport = AggregatedDotnetFormatReport.CreateForSolution(projectOrSolutionPath, report!);
        }
        else
        {
            var projectName = new FileInfo(projectOrSolutionPath).Name;
            aggregatedReport = AggregatedDotnetFormatReport.CreateForProject(projectName, report!);
        }

        return false;
    }


    public static IReadOnlyCollection<Output> DotnetFormatVerifyNoChanges(DotNetFormatSettings? toolSettings = null)
    {
        toolSettings = toolSettings ?? new DotNetFormatSettings();
        var isFormated = DotnetFormatVerifyNoChanges(toolSettings.ProcessWorkingDirectory, toolSettings!.Project!, toolSettings.Include, out var report, out ProcessException? processException);
        if (isFormated is false)
        {
            var outputMessage = CreateOutputMessage(toolSettings.Project!, report!);
            ProcessExceptionHelper.Throw(processException!, outputMessage);

        }
        return new List<Output>();
    }


    public static IReadOnlyCollection<Output> DotnetFormatVerifyNoChanges(Configure<DotNetFormatSettings> configurator)
    {
        return DotnetFormatVerifyNoChanges(configurator(new DotNetFormatSettings()));

    }

    public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> DotnetFormatVerifyNoChanges(CombinatorialConfigure<DotNetFormatSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false)
    {
        return configurator.Invoke(DotnetFormatVerifyNoChanges, DotNetLogger, degreeOfParallelism, completeOnFailure);
    }


    public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> DotnetFormatVerifyNoChanges(GitChangesReport gitChangesReport)
    {
        var reports = ChunkReport(gitChangesReport);

        //return DotnetFormatVerifyNoChanges(_ => _
        //    .SetProcessWorkingDirectory(gitChangesReport.GitRepoLocalDirectory)
        //    .CombineWith(gitChangesReport.SolutionChanges,
        // (_, solutionChange) => _
        //     .SetProject(solutionChange.SolutionFullPath)
        //     .When(solutionChange.IsSolutionChanged, _ => _
        //        .AddInclude(solutionChange.SolutionFullPath))
        //      .CombineWith(solutionChange.ProjectChanges, (_, projectChange) => _
        //              .When(projectChange.IsProjectChanged, _ => _
        //                  .AddInclude(projectChange.ProjectFullPath))
        //            .AddInclude(projectChange.GetAllChangedFiles()))), completeOnFailure: true);

        //return DotnetFormatVerifyNoChanges(_ => _
        //  .SetProcessWorkingDirectory(gitChangesReport.GitRepoLocalDirectory)
        //  .CombineWith(reports, (_, report) => _
        //    .CombineWith(report.SolutionChanges, (_, solution) => _
        //        .SetProject(solution.SolutionFullPath)
        //             .SetProject(solution.SolutionFullPath)
        //             .When(solution.IsSolutionChanged, _ => _
        //                .AddInclude(solution.SolutionFullPath))
        //              .CombineWith(solution.ProjectChanges, (_, projectChange) => _
        //                      .When(projectChange.IsProjectChanged, _ => _
        //                          .AddInclude(projectChange.ProjectFullPath))
        //                    .AddInclude(projectChange.GetAllChangedFiles())))), completeOnFailure: true);

        return DotnetFormatVerifyNoChanges(_ => _
      .SetProcessWorkingDirectory(gitChangesReport.GitRepoLocalDirectory)
      .CombineWith(reports, (_, report) => _
        .CombineWith(report.SolutionChanges, (_, solution) => _
            .SetProject(solution.SolutionFullPath)
                 .SetProject(solution.SolutionFullPath)
                 .When(solution.IsSolutionChanged, _ => _
                    .AddInclude(solution.SolutionFullPath))
                  .AddInclude(solution.ProjectChanges.SelectMany(x => x.GetAllChangedFiles())))),
                  completeOnFailure: true);
    }

    private static GitChangesReport[] ChunkReport(GitChangesReport report)
    {
        List<GitChangesReport> reports = new List<GitChangesReport>();

        foreach (var solution in report.SolutionChanges)
        {
            var chunks = ChunkBy(solution.ProjectChanges.ToList(), 20);
            reports.AddRange(chunks.Select(x => new GitChangesReport(
                report.GitRepoLocalDirectory,
                new[] { new SolutionChanges(solution.SolutionFullPath, solution.IsSolutionChanged, x.ToArray()) })));
        }

        return reports.ToArray();
    }

    private static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    private static string CreateOutputMessage(string project, AggregatedDotnetFormatReport report)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Unformatted documents found in '{project}':");


        foreach (var document in report.Documents)
        {
            stringBuilder.AppendLine($"{document.FileName} required changes: {document.Changes.Length}");
            foreach (var change in document.Changes)
            {
                stringBuilder.AppendLine(change);
            }
            stringBuilder.AppendLine();

        }

        return stringBuilder.ToString();
    }


}