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

    private static Dictionary<string, SolutionProject> CreateSolutionProjectToCsharpProjectMap(string solutionPath)
    {
        static Dictionary<string, SolutionProject> AddAllChildren(
            string? parentCsharpProjectId,
            SolutionFolder folder,
            ref Dictionary<string, SolutionProject> map)
        {
            foreach (var project in folder.Projects)
            {
                string projectId = project.ProjectId.ToString("D");
                map.Add(projectId, new(projectId, ProjectType.Project, project.Name, project.ProjectId.ToString("D")));
            }

            foreach (var nestedFolder in folder.SolutionFolders)
            {
                AddAllChildren(null, nestedFolder, ref map);
            }

            return map;
        }

        var solution = ProjectModelTasks.ParseSolution(solutionPath);
        Dictionary<string, SolutionProject> solutionProjectIdToCsharpProjectMap = new();
        foreach (var project in solution.AllProjects)
        {
            string projectId = project.ProjectId.ToString("D");
            solutionProjectIdToCsharpProjectMap.Add(projectId, new(projectId, ProjectType.Project, project.Name, projectId));
        }

        foreach (var nestedFolder in solution.AllSolutionFolders)
        {
            AddAllChildren(null, nestedFolder, ref solutionProjectIdToCsharpProjectMap);
        }

        return solutionProjectIdToCsharpProjectMap;
    }

    private record TempDocument(string DocumentId, string FilePath, string FileName, string ProjectId, List<string> Changes);

    private record SolutionProject(string Id, ProjectType ProjectType, string Name, string CsharpProjectId);
}

public record AggregatedDocumentReport(string FilePath, string FileName, string ProjectId, string ProjectName, string[] Changes);
