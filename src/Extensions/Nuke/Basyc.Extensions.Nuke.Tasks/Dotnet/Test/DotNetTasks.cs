using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Dotnet.Test;
using Basyc.Extensions.Nuke.Tasks.Dotnet.Test.OpenCover;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System.Globalization;
using System.Xml.Serialization;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{
	private static readonly XmlSerializer xmlSerializer = new(typeof(CoverageSession));

	public static CoverageReport UnitTestAndCoverage(IEnumerable<string> testProjectsPaths, string unitTestProjectNameSuffix = ".UnitTests")
	{
		Dictionary<string, (string fullPath, ProjectCoverageReport? report)>? reportHolder = testProjectsPaths
			.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => (x, (ProjectCoverageReport?)null));

		using var settingsFile = CreateRunSettings(testProjectsPaths, unitTestProjectNameSuffix);
		using var resultsDirectory = TemporaryDirectory.CreateTempDirectory("dotnetTestOutputDir/TestRun", true);
		DotNetTest(_ => _
			.EnableNoRestore()
			.EnableNoBuild()
			.SetResultsDirectory(resultsDirectory.FullPath)
			.SetSettingsFile(settingsFile.FullPath)
			.CombineWith(reportHolder.Values.Select(x => x.fullPath),
		(settings, unitTestProject) => settings
				.SetProjectFile(unitTestProject)),
			degreeOfParallelism: 5);

		var modulesDirs = resultsDirectory.GetInfo().GetDirectories();
		if (modulesDirs.Any() is false || modulesDirs.Length != reportHolder.Keys.Count)
		{
			throw new InvalidOperationException("Coverage results not found");
		}

		//var projectReports = new List<ProjectCoverageReport>();
		for (int moduleIndex = 0; moduleIndex < reportHolder.Keys.Count; moduleIndex++)
		{
			var moduleDir = modulesDirs[moduleIndex];
			string openCoverResults = Path.Combine(moduleDir.FullName, "coverage.opencover.xml");
			using var outputFileStream = System.IO.File.OpenRead(openCoverResults);
			var openCoverCoverageSession = (CoverageSession)xmlSerializer.Deserialize(outputFileStream)!;
			var sessionProjectReports = openCoverCoverageSession.Modules.Module
				.Select(module => new ProjectCoverageReport(
				module.ModuleName,
				 (double)Math.Round(module.Classes.Class
					.Select(x => double.Parse(x.Summary.SequenceCoverage, CultureInfo.InvariantCulture.NumberFormat))
				 .Average()))).ToArray();
			//projectReports.AddRange(sessionProjectReports);
			foreach (var report in sessionProjectReports)
			{
				//reportHolder[report.ProjectName].report = report;
				reportHolder[report.ProjectName + unitTestProjectNameSuffix] = new(report.ProjectName, new ProjectCoverageReport(report.ProjectName, 0));
			}
		}

		Log.Information("Coverage report:");
		//testProjectsPathsArray.Values.ToArray()(x => Log.Information($"Name: {x.ProjectName} SequenceCoverge: {x.SequenceCoverage}"));
		var projectReports = reportHolder.Values.ToArray();
		for (int i = 0; i < projectReports.Length; i++)
		{
			var projectReport = projectReports[i];
			if (projectReport.report is null)
			{
				projectReports[i].report = new ProjectCoverageReport("empty", 0);
			}

			projectReport = projectReports[i];
			Log.Information($"Name: {projectReport!.report!.ProjectName} SequenceCoverge: {projectReport!.report.SequenceCoverage}");
		}

		return new CoverageReport(reportHolder.Values.Select(x => x.report).ToArray()!);
	}

	private static TemporaryFile CreateRunSettings(IEnumerable<string> testProjectPaths, string unitTestProjectNameSuffix)
	{
		string includeParam = string.Join(",", testProjectPaths
			.Select(x =>
			{
				string unitTestProjectName = Path.GetFileNameWithoutExtension(x);
				var sourceProjectName = unitTestProjectName.AsSpan().Slice(0, unitTestProjectName.Length - unitTestProjectNameSuffix.Length);
				return $"[{sourceProjectName}]*";
			}));

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

		var settingFile = TemporaryFile.CreateTempFile("coverlet", "runsettings");
		System.IO.File.WriteAllText(settingFile.FullPath, fileContent);
		return settingFile;
	}
}
