namespace Basyc.Extensions.Nuke.Tasks.Dotnet.Test;

public record ClassCoverageReport(string ClassName, double BranchCoverage, double SequenceCoverage, MethodCoverageReport[] MethodReports);
