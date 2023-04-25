using Nuke.Common.ProjectModel;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Format;

public record AggregatedDotnetFormatReport(List<AggregatedDocumentReport> Documents)
{
    private enum ProjectType
    {
        Project,
        Folder
    }

    public static AggregatedDotnetFormatReport CreateForSolution(string solutionPath, DotnetFormatReport report)
    {
        var documentIdToTempDocumentMap = CreateDocumentMap(report);

        var documents = documentIdToTempDocumentMap.Values
            .Select(x =>
            {
                string projectName = x.FilePath;
                return new AggregatedDocumentReport(x.FilePath, x.FileName, x.ProjectId, projectName, x.Changes.ToArray());
            })
            .ToList();
        return new(documents);
    }

    public static AggregatedDotnetFormatReport CreateForProject(string projectName, DotnetFormatReport report)
    {
        var documentIdToTempDocumentMap = CreateDocumentMap(report);

        var documents = documentIdToTempDocumentMap.Values
            .Select(x => new AggregatedDocumentReport(x.FilePath, x.FileName, x.ProjectId, projectName, x.Changes.ToArray()))
            .ToList();
        return new(documents);
    }

    private static Dictionary<string, TempDocument> CreateDocumentMap(DotnetFormatReport report)
    {
        var documentIdToTempDocumentMap = new Dictionary<string, TempDocument>();
        if (report.Records is null)
        {
            return documentIdToTempDocumentMap;
        }

        foreach (var record in report.Records)
        {
            documentIdToTempDocumentMap.TryAdd(
                record.DocumentId.Id,
                new(record.DocumentId.Id, record.FilePath, record.FileName, record.DocumentId.ProjectId.Id, new()));
            var documentChanges = documentIdToTempDocumentMap[record.DocumentId.Id];
            foreach (var fileChange in record.FileChanges)
            {
                string changeMessage =
                    $"Line:{fileChange.LineNumber} Column:{fileChange.CharNumber} [{fileChange.DiagnosticId}] {fileChange.FormatDescription}";
                documentChanges.Changes.Add(changeMessage);
            }
        }

        return documentIdToTempDocumentMap;
    }

    private sealed record TempDocument(string DocumentId, string FilePath, string FileName, string ProjectId, List<string> Changes);

    private sealed record SolutionProject(string Id, ProjectType ProjectType, string Name, string CsharpProjectId);
}

public record AggregatedDocumentReport(string FilePath, string FileName, string ProjectId, string ProjectName, string[] Changes);
