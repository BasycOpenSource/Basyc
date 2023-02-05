namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public record ProjectCoverageReport(string Name, bool TestProjectFound, bool CoverageExcluded, double BranchCoverage, double SequenceCoverage, ClassCoverageReport[] Classes);
