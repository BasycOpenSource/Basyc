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
			Log.Information($"Files formatted correctly");
			aggregatedReport = null;
			return true;
		}
		else
		{
			Log.Information($"Not formatted file(s) found");
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

	public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> DotnetFormatVerifyNoChanges(GitCompareReport report)
	{
		if (report.CouldCompare is false)
		{
			throw new ArgumentException("Passed invalid report");
		}

		var batchedReport = CreateBatchedReport(report);

		int totalFilesToCheck = batchedReport.Batches.SelectMany(x => x.FilesToInclude).Count();
		Log.Information($"Solutions to check: {report.Solutions.Length}, projects to check: {report.Solutions.Select(x => x.Projects.Length).Sum()}, total files to check: {totalFilesToCheck}. Batching dotnet format into {batchedReport.Batches.Length} batches.");

		return DotnetFormatVerifyNoChanges(_ => _
			.SetProcessWorkingDirectory(report.GitRepoLocalDirectory)
			.CombineWith(batchedReport.Batches, (_, batch) => _
					.SetProject(batch.SolutionPath)
						  .AddInclude(batch.FilesToInclude)),
						  completeOnFailure: true);
	}

	public static IEnumerable<(DotNetFormatSettings Settings, IReadOnlyCollection<Output> Output)> DotnetFormatVerifyNoChanges(params string[] solutionsFullPath)
	{
		return DotnetFormatVerifyNoChanges(_ => _
			.CombineWith(solutionsFullPath, (_, solutionFullPath) => _
				.SetProject(solutionFullPath)));
	}

	private static BatchedReport CreateBatchedReport(GitCompareReport report)
	{
		var batches = new List<ReportBatch>();

		foreach (var solution in report.Solutions)
		{
			string[] changedFilesInSolution = solution.GetChangedFilesFullPath();
			var chunks = ChunkBy(changedFilesInSolution, 250);
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