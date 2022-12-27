using Basyc.Extensions.Nuke.Tasks.Dotnet.Test;
using Nuke.Common.Utilities.Collections;

namespace Basyc.Extensions.Nuke.Tasks;
public class InProgressReport
{
	private readonly Dictionary<string, InProgressProjectReport> map = new();
	public int ProjectCount { get; private set; }
	public InProgressProjectReport GetReport(string projectName)
	{
		return map[projectName];
	}

	public void CompleteReport(string projectName, ProjectCoverageReport report)
	{
		GetReport(projectName).Report = report;
	}

	public void Add(string projectToTestName, string? testProjectPath, bool testProjectFound)
	{
		map.Add(projectToTestName, new InProgressProjectReport(projectToTestName, testProjectPath, testProjectFound));
		ProjectCount += 1;
		if (testProjectFound is false)
		{
			CompleteReport(projectToTestName, new ProjectCoverageReport(projectToTestName, testProjectFound, 0, 0, Array.Empty<ClassCoverageReport>()));
		}
	}

	public void AddRange(IEnumerable<(string projectToTestName, string? testProjectPath, bool testProjectFound)> items)
	{
		items.ForEach(x => Add(x.projectToTestName, x.testProjectPath, x.testProjectFound));
	}

	public InProgressProjectReport[] GetAllReports()
	{
		return map.Values.ToArray();
	}
}

