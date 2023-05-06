namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

#pragma warning disable CA1819 // Properties should not return arrays
public record ProjectCoverageReport(string Name, bool TestProjectFound, bool CoverageExcluded, double BranchCoverage, double SequenceCoverage, ClassCoverageReport[] Classes);
