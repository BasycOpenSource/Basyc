using Newtonsoft.Json;
using Nuke.Common.Tooling;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tasks.Dotnet.Format;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
namespace _build;

public static partial class DotNetTasks
{
    public static bool DotnetFormatVerifyNoChanges(string projectOrSolutionPath, [NotNullWhen(false)] out string? outputMessage, bool throwIfNotFormatted = true)
    {
        var isSolution = projectOrSolutionPath.EndsWith(".sln");
        var formatReportFilePath = Path.GetTempPath() + $"dotnetFormatReport-{DateTime.UtcNow.ToString("dd-MM-hh-mm-ss-fff")}.json";
        bool canBeFormated = false;
        try
        {
            DotNet($"format {projectOrSolutionPath} --verify-no-changes --no-restore --report {formatReportFilePath} --verbosity quiet",
                logOutput: false);

            canBeFormated = false;
            outputMessage = "Code is formated correctly.";
            Log.Information(outputMessage);

        }
        catch (ProcessException)
        {
            DotnetFormatReport dotnetFormatReport = new(JsonConvert.DeserializeObject<ReportRecord[]>(File.ReadAllText(formatReportFilePath)));
            AggregatedDotnetFormatReport aggregatedReport;
            if (isSolution)
            {
                aggregatedReport = AggregatedDotnetFormatReport.CreateForSolution(projectOrSolutionPath, dotnetFormatReport);
            }
            else
            {
                var projectName = new FileInfo(projectOrSolutionPath).Name;
                aggregatedReport = AggregatedDotnetFormatReport.CreateForProject(projectName, dotnetFormatReport);
            }

            canBeFormated = true;
            outputMessage = CreateOutputMessage(aggregatedReport);
            Log.Error(outputMessage);

            if (throwIfNotFormatted)
            {
                throw;
            }
        }
        finally
        {
            File.Delete(formatReportFilePath);
        }


        return canBeFormated;
    }

    public static bool DotnetFormatVerifyNoChanges(IEnumerable<string> projectsOrSolutions, [NotNullWhen(false)] out string? outputMessage, bool throwIfNotFormatted = true)
    {
        bool needsFormating = false;
        List<string?> outputs = new();
        foreach (var project in projectsOrSolutions)
        {
            needsFormating |= DotnetFormatVerifyNoChanges(project, out var innerOutputMessage, false);
            outputs.Add(innerOutputMessage);
        }

        if (throwIfNotFormatted)
            throw new Exception("One of the items needs fomrating. See ouput");

        outputMessage = string.Join("\n", outputs);
        return needsFormating;
    }


    private static string CreateOutputMessage(AggregatedDotnetFormatReport report)
    {
        StringBuilder stringBuilder = new StringBuilder();
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