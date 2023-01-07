using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System.Diagnostics.CodeAnalysis;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks;

[ExcludeFromCodeCoverage]
public static partial class DotNetTasks
{

	public static void BasycBuildAffected(GitCompareReport gitCompareReport, string unitTestSuffix, string buildProjectName, Solution solution)
	{
		using var solutionToUse = SolutionHelper.GetAffectedAsSolution(gitCompareReport, unitTestSuffix, buildProjectName, solution);

		DotNetBuild(_ => _
		 .EnableNoRestore()
		 .SetProjectFile(solutionToUse.Solution));
	}
}
