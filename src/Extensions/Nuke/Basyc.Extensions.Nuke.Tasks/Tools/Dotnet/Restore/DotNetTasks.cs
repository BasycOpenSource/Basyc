using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{

	public static void BasycRestoreAffected(GitCompareReport gitCompareReport, string unitTestSuffix, string buildProjectName, Solution solution)
	{
		using var solutionToUse = SolutionHelper.GetAffectedAsSolution(gitCompareReport, unitTestSuffix, buildProjectName, solution);
		DotNetRestore(_ => _
			.SetProjectFile(solutionToUse.Solution));
	}
}
