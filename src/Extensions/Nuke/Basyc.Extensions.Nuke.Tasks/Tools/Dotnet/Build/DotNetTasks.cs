using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System.Diagnostics.CodeAnalysis;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable once CheckNamespace
namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet;

[ExcludeFromCodeCoverage]
public static partial class DotNetTasks
{
    public static void BasycDotNetBuildAffected(RepositoryChangeReport gitCompareReport, string unitTestSuffix, string buildProjectName, Solution solution)
    {
        using var solutionToUse = TemporarySolution.GetAffectedAsSolution(gitCompareReport, unitTestSuffix, buildProjectName, solution);

        DotNetBuild(_ => _
            .EnableNoRestore()
            .SetProjectFile(solutionToUse.Solution));
    }
}
