using Basyc.Extensions.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.ProjectModel.ProjectModelTasks;

namespace Basyc.Extensions.Nuke.Targets.Helpers.Solutions;
public static class SolutionHelper
{
	/// <summary>
	/// Creates new <see cref="TemporarySolution"/> containing all projects from existing solution exluding build project
	/// </summary>
	/// <param name="solution"></param>
	/// <param name="buildProjectName"></param>
	/// <returns></returns>
	public static TemporarySolution NewTempSolution(Solution solution, string buildProjectName)
	{
		var newSolution = CreateSolution($"{solution.Path.Parent}/global.generated.sln", new[] { solution }, folderNameProvider: x => x == solution ? null : x.Name);
		newSolution.RemoveProject(newSolution.GetProject(buildProjectName));
		newSolution.Save();
		return new TemporarySolution(newSolution);
	}

	/// <summary>
	/// Creates new <see cref="TemporarySolution"/> containing specified projecs from existing solution exluding build project
	/// </summary>
	/// <param name="solution"></param>
	/// <param name="buildProjectName"></param>
	/// <param name="projectsPaths"></param>
	/// <returns></returns>
	public static TemporarySolution NewTempSolution(Solution solution, string buildProjectName, IEnumerable<string> projectsPaths)
	{
		var projectsPathsSet = projectsPaths.ToHashSet();
		var newSolution = CreateSolution($"{solution.Path.Parent}/global.generated.sln", new[] { solution }, folderNameProvider: x => x == solution ? null : x.Name);
		newSolution.AllProjects
			.Where(x => projectsPathsSet.Contains(x.Path.ToString().NormalizePath()) is false)
			.ForEach(newSolution.RemoveProject);
		newSolution.RemoveProject(newSolution.GetProject(buildProjectName));
		newSolution.Save();
		return new TemporarySolution(newSolution);
	}
}
