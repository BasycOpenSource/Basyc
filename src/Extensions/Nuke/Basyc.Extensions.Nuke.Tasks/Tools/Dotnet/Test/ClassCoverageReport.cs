namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

#pragma warning disable CA1819 // Properties should not return arrays
public record ClassCoverageReport(string Name, double BranchCoverage, double SequenceCoverage, MethodCoverageReport[] Methods);
