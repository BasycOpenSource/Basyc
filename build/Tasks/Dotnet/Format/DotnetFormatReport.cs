namespace Tasks.Dotnet.Format;

public record DotnetFormatReport(ReportRecord[]? Records);
public record ReportRecord(DocumentId DocumentId, string FilePath, string FileName, List<FileChange> FileChanges);
public record DocumentId(string Id, ProjectId ProjectId);
public record FileChange(int LineNumber, int CharNumber, string DiagnosticId, string FormatDescription);
public record ProjectId(string Id);
