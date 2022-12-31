using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
public class InProgressReport
{

	private readonly Dictionary<string, InProgressProjectReport> map = new();
	public int ProjectCount { get; private set; }
	public InProgressProjectReport GetReport(string projectName)
	{
		return map[projectName];
	}

	public void Complete(string projectName, ProjectCoverageReport report)
	{
		GetReport(projectName).Report = report;
	}

	public void Add(string projectToTestName, string? testProjectPath, bool testProjectFound)
	{
		map.Add(projectToTestName, new InProgressProjectReport(projectToTestName, testProjectPath, testProjectFound));
		ProjectCount += 1;
		if (testProjectFound is false)
		{
			Complete(projectToTestName, new ProjectCoverageReport(projectToTestName, testProjectFound, 0, 0, Array.Empty<ClassCoverageReport>()));
		}
	}

	public void AddRange(IEnumerable<(string projectToTestName, string? testProjectPath, bool testProjectFound)> items)
	{
		items.ForEach(x => Add(x.projectToTestName, x.testProjectPath, x.testProjectFound));
	}

	public void AddSolution(Solution solution, string testProjectSuffix = ".UnitTests")
	{
		string[] sourceProjectsPaths = solution.AllProjects
			.Where(x => x.Name.EndsWith(testProjectSuffix) is false)
			.Select(x => x.Path.ToString())
			.ToArray();

		AddRange(sourceProjectsPaths.Select(projectToTestPath =>
		{
			string projectName = Path.GetFileNameWithoutExtension(projectToTestPath);
			string unitTestProjectName = Path.GetFileNameWithoutExtension(projectToTestPath) + testProjectSuffix;
			var testProject = solution!.GetProject(unitTestProjectName);
			if (testProject is null)
			{
				return (projectName, null!, false);
			}

			return (projectName, testProject.Path.ToString(), true);
		})!);
	}

	public InProgressProjectReport[] GetAllReports()
	{
		return map.Values.ToArray();
	}
}

