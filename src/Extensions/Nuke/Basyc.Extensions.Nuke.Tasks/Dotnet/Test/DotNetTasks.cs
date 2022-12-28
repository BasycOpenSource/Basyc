using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Dotnet.Test;
using Basyc.Extensions.Nuke.Tasks.Dotnet.Test.OpenCover;
using Basyc.Extensions.Nuke.Tasks.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System.Globalization;
using System.Xml.Serialization;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{
	private static readonly XmlSerializer xmlSerializer = new(typeof(CoverageSession));

	public static CoverageReport UnitTestAndCoverageAffected(Solution solution, GitCompareReport gitCompareReport, string testProjectSuffix = ".UnitTests")
	{
		var projectsToTestPaths = gitCompareReport.ChangedSolutions
			.SelectMany(x => x.ChangedProjects)
			.Select(x => x.ProjectFullPath)
			.Where(x => Path.GetFileNameWithoutExtension(x).EndsWith(testProjectSuffix) is false)
			.ToHashSet();

		string[] changedTestProjectPaths = gitCompareReport.ChangedSolutions
			.SelectMany(x => x.ChangedProjects)
			.Select(x => x.ProjectFullPath)
			.Where(x => Path.GetFileNameWithoutExtension(x).EndsWith(testProjectSuffix))
			.ToArray();

		if (changedTestProjectPaths.Any())
		{
			foreach (string changedTestProjectPath in changedTestProjectPaths)
			{
				string projectName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(changedTestProjectPath));
				string projectPath = solution.GetProject(projectName).Path.ToString().NormalizePath();
				projectsToTestPaths.Add(projectPath);
			}
		}

		return UnitTestAndCoverage(solution, projectsToTestPaths, testProjectSuffix);
	}

	public static CoverageReport UnitTestAndCoverageAll(Solution solution, string testProjectSuffix = ".UnitTests")
	{
		var allUnitTestProjectsPaths = solution!.GetProjects($"!(*{testProjectSuffix})")
				   .Select(x => x.Path.ToString());
		return UnitTestAndCoverage(solution, allUnitTestProjectsPaths, testProjectSuffix);
	}

	private static CoverageReport UnitTestAndCoverage(Solution solution, IEnumerable<string> projectToTestPaths, string textProjectSuffix = ".UnitTests")
	{
		var inProgressReport = new InProgressReport();
		inProgressReport.AddRange(projectToTestPaths.Select(projectToTestPath =>
		{
			string projectName = Path.GetFileNameWithoutExtension(projectToTestPath);
			string unitTestProjectName = Path.GetFileNameWithoutExtension(projectToTestPath) + textProjectSuffix;
			var testProject = solution!.GetProject(unitTestProjectName);
			if (testProject is null)
			{
				return (projectName, null!, false);
			}

			return (projectName, testProject.Path.ToString(), true);
		})!);

		var testProjectsToRun = inProgressReport.GetAllReports().Where(x => x.TestProjectFound).ToArray();
		using var settingsFile = CreateRunSettings(testProjectsToRun.Select(x => x.ProjectToTestName));
		using var projectResultDirectory = TemporaryDirectory.CreateNew("BasycDotnetTest/Projects");
		DotNetTest(_ => _
			.EnableNoRestore()
			.EnableNoBuild()
			.EnableCollectCoverage()
			.SetSettingsFile(settingsFile.FullPath)
			.CombineWith(testProjectsToRun,
		(settings, projectReport) => settings
				.SetResultsDirectory(projectResultDirectory.FullPath + "/" + projectReport.ProjectToTestName)
				.SetProjectFile(projectReport.TestProjectPath)),
			degreeOfParallelism: 5);

		foreach (var testProjectReport in testProjectsToRun)
		{
			var dir = new DirectoryInfo(projectResultDirectory.FullPath + "/" + testProjectReport.ProjectToTestName);
			var uniqueNameDir = dir.GetDirectories().First();

			string openCoverResults = Path.Combine(uniqueNameDir.FullName, "coverage.opencover.xml");
			using var outputFileStream = System.IO.File.OpenRead(openCoverResults);
			var openCoverCoverageSession = (CoverageSession)xmlSerializer.Deserialize(outputFileStream)!;
			var openCoverReport = openCoverCoverageSession.Modules.Module
				.Select(ParseOpencoverModule).FirstOrDefault();

			if (openCoverReport == default)
			{
				//When parsing a open cover file returns 0 modules.
				//It means that there are 0 tests inside the test project
				//that is testing project to test.
				inProgressReport.CompleteReport(testProjectReport.ProjectToTestName, new ProjectCoverageReport(testProjectReport.ProjectToTestName, true, 0, 0, Array.Empty<ClassCoverageReport>()));
			}
			else
			{
				inProgressReport.CompleteReport(testProjectReport.ProjectToTestName, openCoverReport);
			}
		}

		LogTestReport(inProgressReport);

		return new CoverageReport(inProgressReport.GetAllReports().Select(x => x.Report!).ToArray());
	}

	private static void LogTestReport(InProgressReport inProgressReport)
	{
		Log.Information("Coverage report:");
		var projectReports = inProgressReport.GetAllReports();
		foreach (var projectReport in projectReports)
		{
			Log.Information($"		Assembly: {projectReport.ProjectToTestName} BranchCoverage: {projectReport!.Report!.BranchCoverage}% SequenceCoverage: {projectReport.Report.SequenceCoverage}% TestFound: {projectReport!.Report!.TestProjectFound}");

			foreach (var classReport in projectReport.Report.ClassReports)
			{
				Log.Information($"			Class: {classReport.ClassName} BranchCoverage: {classReport.BranchCoverage}% SequenceCoverage: {classReport.SequenceCoverage}%");
				foreach (var methodReport in classReport.MethodReports)
				{
					Log.Information($"				Method: {classReport.ClassName} BranchCoverage: {methodReport.BranchCoverage}% SequenceCoverage: {methodReport.SequenceCoverage}%");
				}
			}
		}
	}

	private static TemporaryFile CreateRunSettings(IEnumerable<string> projectToTestNames)
	{
		string includeParam = string.Join(",", projectToTestNames.Select(x => $"[{x}]*"));

		//Example and more options here:
		//https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
		string fileContent = $"""
			<?xml version="1.0" encoding="utf-8" ?>
			<RunSettings>
			  <DataCollectionRunSettings>
			    <DataCollectors>
			      <DataCollector friendlyName="XPlat code coverage">
			        <Configuration>
			          <Format>opencover</Format>          
					  <Include>{includeParam}</Include> <!-- [Assembly-Filter]Type-Filter -->
			          <Exclude>[*test*]*</Exclude> <!-- [Assembly-Filter]Type-Filter -->
			          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
			          <SingleHit>false</SingleHit>
			          <UseSourceLink>true</UseSourceLink>
			          <IncludeTestAssembly>true</IncludeTestAssembly>
			          <SkipAutoProps>true</SkipAutoProps>
			          <DeterministicReport>false</DeterministicReport>
			          <ExcludeAssembliesWithoutSources>MissingAll,MissingAny,None</ExcludeAssembliesWithoutSources>
			        </Configuration>
			      </DataCollector>
			    </DataCollectors>
			  </DataCollectionRunSettings>
			</RunSettings>
			""";

		var settingFile = TemporaryFile.CreateNew("coverlet", "runsettings");
		System.IO.File.WriteAllText(settingFile.FullPath, fileContent);
		return settingFile;
	}

	private static string GetTestedProjectName(string testProjectPath, string unitTestProjectNameSuffix)
	{
		var sourceProjectName = Path.GetFileNameWithoutExtension(testProjectPath).AsSpan().Slice(0, testProjectPath.Length - unitTestProjectNameSuffix.Length);
		return $"{sourceProjectName}";
	}

	private static bool TryGetTestProjectPath(string projectToTestPath, Solution solution, string testProjectSuffix, out string? testProjectPath)
	{
		string unitTestProjectName = Path.GetFileNameWithoutExtension(projectToTestPath) + testProjectSuffix;
		var unitTestProject = solution!.GetProject(unitTestProjectName);
		if (unitTestProject is null)
		{
			testProjectPath = null;
			return false;
		}

		testProjectPath = unitTestProject.Path.ToString().Replace('\\', '/');
		return true;
	}

	private static ProjectCoverageReport ParseOpencoverModule(Module module)
	{
		double sequenceCoverage = (double)Math.Round(module.Classes.Class
			.Select(x => double.Parse(x.Summary.SequenceCoverage, CultureInfo.InvariantCulture.NumberFormat))
			.Average());

		return new ProjectCoverageReport(
				module.ModuleName,
				true,
				 double.NaN,
				 double.NaN,
				 module.Classes.Class
				 .Select(classDto =>
				 {
					 string className = Path.GetFileNameWithoutExtension(classDto.FullName + ".cs");
					 return new ClassCoverageReport(className, ParseDouble(classDto.Summary.BranchCoverage), ParseDouble(classDto.Summary.SequenceCoverage), classDto.Methods.Method
						 .Select(methodDto => new MethodCoverageReport(methodDto.Name, ParseDouble(methodDto.BranchCoverage), ParseDouble(methodDto.SequenceCoverage)))
						 .ToArray());
				 })
				 .ToArray());
	}

	private static double ParseDouble(string nubmer)
	{
		return double.Parse(nubmer, CultureInfo.InvariantCulture.NumberFormat);
	}
}

