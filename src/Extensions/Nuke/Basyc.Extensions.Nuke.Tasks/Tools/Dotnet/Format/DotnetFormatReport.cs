namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Format;

#pragma warning disable CA1819 // Properties should not return arrays
public record DotnetFormatReport(ReportRecord[]? Records);
public record ReportRecord(DocumentId DocumentId, string FilePath, string FileName, FileChange[] FileChanges);
public record DocumentId(string Id, ProjectId ProjectId);
public record FileChange(int LineNumber, int CharNumber, string DiagnosticId, string FormatDescription);
public record ProjectId(string Id);
