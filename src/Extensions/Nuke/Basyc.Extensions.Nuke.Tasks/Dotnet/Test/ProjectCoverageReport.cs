namespace Basyc.Extensions.Nuke.Tasks.Dotnet.Test;

public record ProjectCoverageReport(string ProjectName, bool TestProjectFound, double BranchCoverage, double SequenceCoverage, ClassCoverageReport[] ClassReports);
