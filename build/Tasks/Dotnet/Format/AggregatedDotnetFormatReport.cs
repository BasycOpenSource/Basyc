using Nuke.Common.ProjectModel;

namespace Tasks.Dotnet.Format
{
    public record AggregatedDotnetFormatReport(List<AggregatedDocumentReport> Documents)
    {
        private record TempDocument(string DocumentId, string FilePath, string FileName, string ProjectId, List<string> Changes);
        private record SolutionProject(string Id, ProjectType ProjectType, string Name, string CsharpProjectId);
        private enum ProjectType { Project, Folder }
        //private record Project(string Id, string ProjectType, string Name, string ParentProjectId);
        public static AggregatedDotnetFormatReport CreateForSolution(string solutionPath, DotnetFormatReport report)
        {
            Dictionary<string, TempDocument> documentIdToTempDocumentMap = CreateDocumentMap(report);
            //var projectIdToProjectNameMap = CreateSolutionProjectToCsharpProjectMap(solutionPath);
            //var projectIdToProjectNameMap = new Dictionary<string, Project>();

            var documents = documentIdToTempDocumentMap.Values
                .Select(x =>
                {
                    //var projectName = projectIdToProjectNameMap[x.ProjectId].Name;
                    //var projectName = "dummy";
                    var projectName = x.FilePath;
                    return new AggregatedDocumentReport(x.FilePath, x.FileName, x.ProjectId, projectName, x.Changes.ToArray());
                })
                .ToList();
            return new AggregatedDotnetFormatReport(documents);
        }

        public static AggregatedDotnetFormatReport CreateForProject(string projectName, DotnetFormatReport report)
        {
            Dictionary<string, TempDocument> documentIdToTempDocumentMap = CreateDocumentMap(report);

            var documents = documentIdToTempDocumentMap.Values
                .Select(x => new AggregatedDocumentReport(x.FilePath, x.FileName, x.ProjectId, projectName, x.Changes.ToArray()))
                .ToList();
            return new AggregatedDotnetFormatReport(documents);
        }

        private static Dictionary<string, TempDocument> CreateDocumentMap(DotnetFormatReport report)
        {
            var documentIdToTempDocumentMap = new Dictionary<string, TempDocument>();
            foreach (var record in report.Records)
            {
                documentIdToTempDocumentMap.TryAdd(record.DocumentId.Id, new(record.DocumentId.Id, record.FilePath, record.FileName, record.DocumentId.ProjectId.Id, new List<string>()));
                var documentChanges = documentIdToTempDocumentMap[record.DocumentId.Id];
                foreach (var fileChange in record.FileChanges)
                {
                    string changeMessage = $"Line:{fileChange.LineNumber} Column:{fileChange.CharNumber} [{fileChange.DiagnosticId}] {fileChange.FormatDescription}";
                    documentChanges.Changes.Add(changeMessage);
                }
            }

            return documentIdToTempDocumentMap;
        }

        private static Dictionary<string, SolutionProject> CreateSolutionProjectToCsharpProjectMap(string solutionPath)
        {
            static Dictionary<string, SolutionProject> AddAllChildren(string? parentCsharpProjectId, SolutionFolder folder, ref Dictionary<string, SolutionProject> map)
            {
                foreach (var project in folder.Projects)
                {
                    var projectId = project.ProjectId.ToString("D");
                    map.Add(projectId, new SolutionProject(projectId, ProjectType.Project, project.Name, project.ProjectId.ToString("D")));
                }

                foreach (SolutionFolder nestedFolder in folder.SolutionFolders)
                {
                    AddAllChildren(null, nestedFolder, ref map);
                }
                return map;
            }

            var solution = ProjectModelTasks.ParseSolution(solutionPath);
            Dictionary<string, SolutionProject> solutionProjectIdToCsharpProjectMap = new();
            foreach (var project in solution.AllProjects)
            {
                //var msProject = project.GetMSBuildProject();
                //var t = msProject.AllEvaluatedItemDefinitionMetadata;
                //var tt = msProject.AllEvaluatedItems;
                //var ttt = msProject.AllEvaluatedProperties;
                //var tttt = msProject.GetAllGlobs();
                //var ttttt = msProject.GetLogicalProject();
                //var tttttt = msProject.ItemDefinitions;
                //var ttttttt = msProject.Items;
                //var tttttttt = msProject.ItemTypes;
                //var ttttttttt = msProject.ProjectCollection;
                //var tttttttttt = msProject.Properties;
                var projectId = project.ProjectId.ToString("D");
                solutionProjectIdToCsharpProjectMap.Add(projectId, new SolutionProject(projectId, ProjectType.Project, project.Name, projectId));
            }

            foreach (SolutionFolder nestedFolder in solution.AllSolutionFolders)
            {
                AddAllChildren(null, nestedFolder, ref solutionProjectIdToCsharpProjectMap);
            }

            return solutionProjectIdToCsharpProjectMap;
        }

        //private static Dictionary<string, SolutionProject> CreateSolutionProjectToCsharpProjectMap2(string solutionPath)
        //{
        //    static Dictionary<string, SolutionProject> AddAllChildren(string? parentCsharpProjectId, SolutionFolder folder, ref Dictionary<string, SolutionProject> map)
        //    {
        //        foreach (var project in folder.Projects)
        //        {
        //            var projectId = project.ProjectId.ToString("D");
        //            map.Add(projectId, new SolutionProject(projectId, ProjectType.Project, project.Name, project.ProjectId.ToString("D")));
        //        }

        //        foreach (SolutionFolder nestedFolder in folder.SolutionFolders)
        //        {
        //            AddAllChildren(null, nestedFolder, ref map);
        //        }
        //        return map;
        //    }

        //    var solution = SolutionFile.Parse(solutionPath);
        //    Dictionary<string, SolutionProject> solutionProjectIdToCsharpProjectMap = new();
        //    foreach (var project in solution.ProjectsInOrder)
        //    {
        //        switch (project.ProjectType)
        //        {
        //            case SolutionProjectType.KnownToBeMSBuildFormat:
        //                {
        //                    break;
        //                }
        //            case SolutionProjectType.SolutionFolder:
        //                {
        //                    break;
        //                }
        //        }
        //    }

        //    return solutionProjectIdToCsharpProjectMap;
        //}


    }

    public record AggregatedDocumentReport(string FilePath, string FileName, string ProjectId, string ProjectName, string[] Changes);
}