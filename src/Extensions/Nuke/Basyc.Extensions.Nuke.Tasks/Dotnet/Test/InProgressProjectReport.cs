using Basyc.Extensions.Nuke.Tasks.Dotnet.Test;

namespace Basyc.Extensions.Nuke.Tasks;
public class InProgressProjectReport
{
	public InProgressProjectReport(string projectToTestName, string? testProjectPath, bool testProjectFound)
	{
		ProjectToTestName = projectToTestName;
		TestProjectPath = testProjectPath;
		TestProjectFound = testProjectFound;
	}

	public string ProjectToTestName { get; }
	public string? TestProjectPath { get; }
	public bool TestProjectFound { get; }
	public ProjectCoverageReport? Report { get; set; }
}

