namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public record ProjectCoverageReport(string ProjectName, bool TestProjectFound, double BranchCoverage, double SequenceCoverage, ClassCoverageReport[] ClassReports);
