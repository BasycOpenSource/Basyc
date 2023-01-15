namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public record ClassCoverageReport(string Name, double BranchCoverage, double SequenceCoverage, MethodCoverageReport[] Methods);
