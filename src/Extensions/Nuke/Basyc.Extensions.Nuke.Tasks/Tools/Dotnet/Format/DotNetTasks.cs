using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Format;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.Tooling;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable CheckNamespace
namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet;

public static partial class DotNetTasks
{
    public static IReadOnlyCollection<Output> DotnetFormatVerifyNoChanges(DotNetFormatSettings? toolSettings = null)
    {
        toolSettings ??= new();
        bool isFormatted = DotnetFormatVerifyNoChanges(toolSettings.ProcessWorkingDirectory,
            toolSettings!.Project!,
            toolSettings.Include,
            out var report,
            out var processException);
        if (isFormatted is false)
        {
            string[] outputMessages = CreateOutputMessages(toolSettings.Project!, report!);
            ProcessExceptionHelper.Throw(processException!, outputMessages);
        }

        return new List<Output>();
    }

    public static IReadOnlyCollection<Output> DotnetFormatVerifyNoChanges(Configure<DotNetFormatSettings> configurator) =>
        DotnetFormatVerifyNoChanges(configurator(new()));

    public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> DotnetFormatVerifyNoChanges(
        CombinatorialConfigure<DotNetFormatSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false) =>
        configurator.Invoke(DotnetFormatVerifyNoChanges, DotNetLogger, degreeOfParallelism, completeOnFailure);

    public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> BasycDotNetFormatVerifyNoChangesAffected(
        RepositoryChangeReport report)
    {
        if (report.CouldCompare is false)
            throw new ArgumentException("Passed invalid report");

        var batchedReport = CreateBatchedReport(report);

        int totalFilesToCheck = batchedReport.Batches.SelectMany(x => x.FilesToInclude).Count();
        Log.Information(
            $"Solutions to check: {report.ChangedSolutions.Length}, projects to check: {report.ChangedSolutions.Select(x => x.ChangedProjects.Length).Sum()}, total files to check: {totalFilesToCheck}. Batching dotnet format into {batchedReport.Batches.Length} batches.");

        return DotnetFormatVerifyNoChanges(_ => _
                .SetProcessWorkingDirectory(report.GitRepoLocalDirectory)
                .CombineWith(batchedReport.Batches, (_, batch) => _
                    .SetProject(batch.SolutionPath)
                    .AddInclude(batch.FilesToInclude)),
            completeOnFailure: true);
    }

    public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> BasycFormatVerifyNoChanges(params string[] solutionsFullPath)
        => DotnetFormatVerifyNoChanges(_ => _
            .CombineWith(solutionsFullPath, (_, solutionFullPath) => _.SetProject(solutionFullPath)));

    private static bool DotnetFormatVerifyNoChanges(
        string workingDirectory,
        string projectOrSolutionPath,
        IEnumerable<string> filesToCheck,
        [NotNullWhen(false)] out AggregatedDotnetFormatReport? aggregatedReport,
        out ProcessException? processException)
    {
        using var fix = FormatWithStyleEnforceFix.Fix(projectOrSolutionPath);
        bool isFormatted = DotnetCliWrapper.FormatVerifyNoChanges(workingDirectory, projectOrSolutionPath, filesToCheck, out var report, out processException);
        fix.Dispose();
        if (isFormatted)
        {
            Log.Information("Files formatted correctly");
            aggregatedReport = null;
            return true;
        }

        Log.Information("Not formatted file(s) found");

        bool isSolution = projectOrSolutionPath.EndsWith(".sln");

        if (isSolution)
        {
            aggregatedReport = AggregatedDotnetFormatReport.CreateForSolution(report!);
        }
        else
        {
            string projectName = new FileInfo(projectOrSolutionPath).Name;
            aggregatedReport = AggregatedDotnetFormatReport.CreateForProject(projectName, report!);
        }

        return false;
    }

    private static BatchedReport CreateBatchedReport(RepositoryChangeReport report)
    {
        var batches = new List<ReportBatch>();

        foreach (var solution in report.ChangedSolutions)
        {
            string[] changedFilesInSolution = solution.GetChangedFilesFullPath();
            var chunks = ChunkBy(changedFilesInSolution, 250);
            batches.AddRange(chunks.Select(x => new ReportBatch(solution.SolutionFullPath, x.ToArray())));
        }

        return new(batches.ToArray());
    }

    private static List<List<T>> ChunkBy<T>(this ICollection<T> source, int chunkSize) => source
        .Select((x, i) => new
        {
            Index = i,
            Value = x
        })
        .GroupBy(x => x.Index / chunkSize)
        .Select(x => x.Select(v => v.Value).ToList())
        .ToList();

    private static string[] CreateOutputMessages(string project, AggregatedDotnetFormatReport report)
    {
        List<string> errorMessages = new();
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Unformatted documents found in '{project}':");

        foreach (var document in report.Documents)
        {
            stringBuilder.AppendLine($"{document.FileName} required changes: {document.Changes.Count}");
            foreach (string change in document.Changes)
                stringBuilder.AppendLine(change);

            errorMessages.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        return errorMessages.ToArray();
    }

    private sealed record BatchedReport(ReportBatch[] Batches);

    private sealed record ReportBatch(string SolutionPath, string[] FilesToInclude);
}
