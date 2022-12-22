using Nuke.Common.Tooling;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tasks.Dotnet;
using Tasks.Dotnet.Format;
using Tasks.Git.Diff;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
namespace _build;

public static partial class DotNetTasks
{
    private record BatchedReport(ReportBatch[] Batches);
    private record ReportBatch(string SolutionPath, string[] FilesToInclude);

    private static bool DotnetFormatVerifyNoChanges(string workingDirectory, string projectOrSolutionPath, IEnumerable<string> filesTocheck, [NotNullWhen(false)] out AggregatedDotnetFormatReport? aggregatedReport, out ProcessException? processException)
    {
        bool isFormated = DotnetWrapper.FormatVerifyNoChanges(workingDirectory, projectOrSolutionPath, filesTocheck, out var report, out processException);
        if (isFormated)
        {
            if (filesTocheck is null || filesTocheck.Any() is false)
            {
                Log.Information($"Project '{projectOrSolutionPath}' is formatted.");
            }
            else
            {
                string filesToCheckString = string.Join("\n", filesTocheck);
                Log.Information($"These files or directories were checked are formatted correctly:\n{filesToCheckString}");
            }

            aggregatedReport = null;
            return true;
        }

        bool isSolution = projectOrSolutionPath.EndsWith(".sln");

        if (isSolution)
        {
            aggregatedReport = AggregatedDotnetFormatReport.CreateForSolution(projectOrSolutionPath, report!);
        }
        else
        {
            string projectName = new FileInfo(projectOrSolutionPath).Name;
            aggregatedReport = AggregatedDotnetFormatReport.CreateForProject(projectName, report!);
        }

        return false;
    }

    public static IReadOnlyCollection<Output> DotnetFormatVerifyNoChanges(DotNetFormatSettings? toolSettings = null)
    {
        toolSettings ??= new DotNetFormatSettings();
        bool isFormated = DotnetFormatVerifyNoChanges(toolSettings.ProcessWorkingDirectory, toolSettings!.Project!, toolSettings.Include, out var report, out var processException);
        if (isFormated is false)
        {

            string[] outputMessages = CreateOutputMessages(toolSettings.Project!, report!);
            ProcessExceptionHelper.Throw(processException!, outputMessages);
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
        var batchedReport = CreateBatchedReport(gitChangesReport);
        int totalFilesToCheck = batchedReport.Batches.SelectMany(x => x.FilesToInclude).Select(x => x.Length).Sum();
        Log.Information($"Solution to check: {gitChangesReport.SolutionChanges.Length}, projects to check: {gitChangesReport.SolutionChanges.Select(x => x.ProjectChanges.Length).Sum()}, total files to check: {totalFilesToCheck}. Batching dotnet format into {batchedReport.Batches.Length} batches.");
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

        //  return DotnetFormatVerifyNoChanges(_ => _
        //.SetProcessWorkingDirectory(gitChangesReport.GitRepoLocalDirectory)
        //.CombineWith(reports, (_, report) => _
        //  .CombineWith(report.SolutionChanges, (_, solution) => _
        //      .SetProject(solution.SolutionFullPath)
        //           .SetProject(solution.SolutionFullPath)
        //           .When(solution.IsSolutionChanged, _ => _
        //              .AddInclude(solution.SolutionFullPath))
        //            .AddInclude(solution.ProjectChanges.SelectMany(x => x.GetAllChangedFiles())))),
        //            completeOnFailure: true);

        return DotnetFormatVerifyNoChanges(_ => _
      .SetProcessWorkingDirectory(gitChangesReport.GitRepoLocalDirectory)
      .CombineWith(batchedReport.Batches, (_, batch) => _
            .SetProject(batch.SolutionPath)
                  .AddInclude(batch.FilesToInclude)),
                  completeOnFailure: true);
    }

    //private static GitChangesReport[] ChunkReport(GitChangesReport report)
    //{
    //    List<GitChangesReport> reports = new List<GitChangesReport>();

    //    foreach (var solution in report.SolutionChanges)
    //    {
    //        var chunks = ChunkBy(solution.ProjectChanges.ToList(), 20);
    //        reports.AddRange(chunks.Select(x => new GitChangesReport(
    //            report.GitRepoLocalDirectory,
    //            new[] { new SolutionChanges(solution.SolutionFullPath, solution.IsSolutionChanged, x.ToArray()) })));
    //    }

    //    return reports.ToArray();
    //}

    private static BatchedReport CreateBatchedReport(GitChangesReport report)
    {
        var batches = new List<ReportBatch>();

        foreach (var solution in report.SolutionChanges)
        {
            string[] changedFilesInSolution = solution.ProjectChanges
                            .SelectMany(x => x.GetAllChangedFiles())
                            .Concat(solution.IsSolutionChanged ? new[] { solution.SolutionFullPath } : Enumerable.Empty<string>())
                            .ToArray();

            var chunks = ChunkBy(changedFilesInSolution, 150);
            batches.AddRange(chunks.Select(x => new ReportBatch(solution.SolutionFullPath, x.ToArray())));
        }

        return new BatchedReport(batches.ToArray());
    }

    private static List<List<T>> ChunkBy<T>(this ICollection<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    private static string[] CreateOutputMessages(string project, AggregatedDotnetFormatReport report)
    {
        List<string> errorMessages = new();
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Unformatted documents found in '{project}':");

        foreach (var document in report.Documents)
        {
            stringBuilder.AppendLine($"{document.FileName} required changes: {document.Changes.Length}");
            foreach (string change in document.Changes)
            {
                stringBuilder.AppendLine(change);
            }

            errorMessages.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        return errorMessages.ToArray();
    }
}

