namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public class InProgressProjectReport
{
    public InProgressProjectReport(string projectToTestName, string? testProjectPath, bool testProjectFound, bool coverageExcluded)
    {
        ProjectToTestName = projectToTestName;
        TestProjectPath = testProjectPath;
        TestProjectFound = testProjectFound;
        CoverageExcluded = coverageExcluded;
    }

    public string ProjectToTestName { get; }

    public string? TestProjectPath { get; }

    public bool TestProjectFound { get; }

    public bool CoverageExcluded { get; }

    public ProjectCoverageReport? Report { get; set; }
}
