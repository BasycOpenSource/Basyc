namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public record ClassCoverageReport(string ClassName, double BranchCoverage, double SequenceCoverage, MethodCoverageReport[] MethodReports);
