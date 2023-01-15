using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.File;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Palmmedia.ReportGenerator.Core;
using Serilog;
using Spectre.Console;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{
	private static readonly XmlSerializer xmlSerializer = new(typeof(CoverageSession));

	public static CoverageReport BasycUnitTestAffected(Solution solution, AffectedReport gitCompareReport, string testProjectSuffix = ".UnitTests")
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

		return UnitTest(solution, projectsToTestPaths, testProjectSuffix);
	}

	public static CoverageReport BasycUnitTestAll(Solution solution, string testProjectSuffix = ".UnitTests")
	{
		var sourceProjects = solution.AllProjects
			.Where(x => x.Name.EndsWith(testProjectSuffix) is false)
			.Select(x => x.Path.ToString());
		return UnitTest(solution, sourceProjects, testProjectSuffix);
	}

	public static void BasycTestAssertMinimum(CoverageReport report, double minSequenceCoverage, double minBranchCoverage)
	{
		const string indent = "  ";
		List<string> errors = new();
		var classErrorStringBuilder = new StringBuilder();
		report.Projects.ForEach(module =>
		{

			if (module.CoverageExcluded)
			{
				return;
			}

			classErrorStringBuilder.AppendLine($"Project '{module.Name}'");
			module.Classes.ForEach(classReport =>
			{
				var classErrors = new List<string>();
				if (classReport.SequenceCoverage < minSequenceCoverage)
				{
					classErrors.Add($"Sequence coverage {classReport.SequenceCoverage}% should be {minSequenceCoverage}%");
				}

				if (classReport.BranchCoverage < minBranchCoverage)
				{
					classErrors.Add($"Branch coverage {classReport.BranchCoverage}% should be {minBranchCoverage}%");
				}

				if (classErrors.Any())
				{

					classErrorStringBuilder.Append(indent);
					classErrorStringBuilder.Append($"Class '{classReport.Name}' ");
					classErrors.ForEach(classError =>
					{
						classErrorStringBuilder.Append(classError);
						classErrorStringBuilder.Append(" ");
					});
				}

				if (classReport.Methods.Any())
				{
					classErrorStringBuilder.AppendLine();
				}

				classReport.Methods.ForEach(method =>
				{
					var methodErrors = new List<string>();

					if (method.SequenceCoverage < minSequenceCoverage)
					{
						methodErrors.Add($"Sequence coverage {method.SequenceCoverage}% should be {minSequenceCoverage}%.");
					}

					if (method.BranchCoverage < minBranchCoverage)
					{
						methodErrors.Add($"Branch coverage {method.BranchCoverage}% should be {minBranchCoverage}%.");
					}

					if (methodErrors.Any())
					{
						classErrorStringBuilder.Append(indent);
						classErrorStringBuilder.Append(indent);
						int methodNameIndex = method.Name.IndexOf("::") + 2;
						string shortMethodName = method.Name.Substring(methodNameIndex);
						classErrorStringBuilder.Append($"Method '{shortMethodName}' ");
						methodErrors.ForEach(methodError =>
						{
							classErrorStringBuilder.Append(methodError);
							classErrorStringBuilder.Append(" ");
						});
					}

					classErrorStringBuilder.AppendLine();
				});
				classErrorStringBuilder.AppendLine();
			});
			errors.Add(classErrorStringBuilder.ToString());
			classErrorStringBuilder.Clear();
		});

		if (errors.Any())
		{
			throw new Exception("Some projects do not meet coverage minimum.");
		}
	}

	public static CoverageSummary BasycTestCreateSummaryMarkdown(CoverageReport coverageReport)
	{
		string[] coverageFilePathPatterns = coverageReport.ProjectToCoverageFileMap.Select(x => x.Value.FullPath).ToArray();
		var summaryOutputDir = TemporaryDirectory.CreateNew($"{nameof(BasycTestCreateSummaryMarkdown)}/CoverageSummary");
		var historyDir = Directory.CreateDirectory(TemporaryDirectory.GetNewPath($"{nameof(BasycTestCreateSummaryMarkdown)}/CoverageHistory", false));
		var summaryGenerator = new Palmmedia.ReportGenerator.Core.Generator();
		summaryGenerator.GenerateReport(new ReportConfiguration(
			coverageFilePathPatterns,
			summaryOutputDir.FullPath,
			Enumerable.Empty<string>(),
			historyDir.FullName,
			new[] { "MarkdownSummaryGithub", "MarkdownDeltaSummary", "JsonSummary", "Html", "TextSummary", "TextDeltaSummary" },
			Enumerable.Empty<string>(),
			Enumerable.Empty<string>(),
			Enumerable.Empty<string>(),
			Enumerable.Empty<string>(),
			"Info",
			null));

		return new CoverageSummary(summaryOutputDir.FullPath);
	}

	public static void BasycTestCreateSummaryConsole(CoverageReport coverageReport, double minSequenceCoverage, double minBranchCoverage, CoverageReport? oldCoverageReport = null)
	{

		string goodText = "good";
		string goodColor = "green1";
		string badText = "bad";
		string badColor = "red1";

		var assemblyTable = new Table();
		assemblyTable.AddColumn("file");
		assemblyTable.AddColumn(new TableColumn("branch old").RightAligned());
		assemblyTable.AddColumn(new TableColumn("branch").RightAligned());
		assemblyTable.AddColumn(new TableColumn("line old").RightAligned());
		assemblyTable.AddColumn(new TableColumn("line").RightAligned());
		assemblyTable.AddColumn(new TableColumn("status").LeftAligned());

		foreach (var project in coverageReport.Projects)
		{

			bool projectBranchBad = project.BranchCoverage < minBranchCoverage;
			bool projectSequenceBad = project.SequenceCoverage < minSequenceCoverage;
			bool projectIsBad = projectBranchBad || projectSequenceBad;
			string projectColor = projectIsBad ? badColor : goodColor;
			string projectBranchColor = projectBranchBad ? badColor : goodColor;
			string projectSequenceColor = projectSequenceBad ? badColor : goodColor;
			string projectstatusText = projectIsBad ? badText : goodText;
			string projectNameTag = project.TestProjectFound ? "" : $"[{badColor}](test not found)[/]";

			ProjectCoverageReport? oldProject = null;
			var oldProjectbranchCoverageText = Markup.FromInterpolated($"");
			var oldProjectCoverageText = Markup.FromInterpolated($"");
			if (oldCoverageReport is not null)
			{
				oldProject = oldCoverageReport.Projects.FirstOrDefault(x => x.Name == project.Name);
				if (oldProject is not null)
				{
					bool oldProjectBranchBad = oldProject.BranchCoverage < minBranchCoverage;
					bool oldProjectSequenceBad = oldProject.SequenceCoverage < minSequenceCoverage;
					string oldProjectBranchColor = oldProjectBranchBad ? badColor : goodColor;
					string oldProjectSequenceColor = oldProjectSequenceBad ? badColor : goodColor;
					oldProjectbranchCoverageText = Markup.FromInterpolated($"[{oldProjectBranchColor}] {oldProject.BranchCoverage}%[/]");
					oldProjectCoverageText = Markup.FromInterpolated($"[{oldProjectSequenceColor}]{oldProject.SequenceCoverage}%[/]");
				}
			}

			var priojectNameText = new Markup($"[bold]{project.Name}.csproj[/] {projectNameTag}");
			var projectBranchCoverageText = Markup.FromInterpolated($"[{projectBranchColor}] {project.BranchCoverage}%[/]");
			var projectCoverageText = Markup.FromInterpolated($"[{projectSequenceColor}]{project.SequenceCoverage}%[/]");
			var projectStatusMarkup = Markup.FromInterpolated($"[{projectColor} bold]{projectstatusText}[/]");

			assemblyTable.AddRow(priojectNameText, oldProjectbranchCoverageText, projectBranchCoverageText, oldProjectCoverageText, projectCoverageText, projectStatusMarkup);

			foreach (var @class in project.Classes)
			{
				bool classBranchBad = @class.BranchCoverage < minBranchCoverage;
				bool classSequenceBad = @class.SequenceCoverage < minSequenceCoverage;
				bool classIsBad = classBranchBad || classSequenceBad;
				string classColor = classIsBad ? badColor : goodColor;
				string classBranchColor = classBranchBad ? badColor : goodColor;
				string classSequenceColor = classSequenceBad ? badColor : goodColor;
				string classStatusText = classIsBad ? badText : goodText;

				var oldClassBranchCoverageMarkup = Markup.FromInterpolated($"");
				var oldClassCoverageMarkup = Markup.FromInterpolated($"");
				ClassCoverageReport? oldClass = null;
				if (oldProject is not null)
				{
					oldClass = project.Classes.FirstOrDefault(x => x.Name == @class.Name);
					if (oldClass is not null)
					{
						bool oldClassBranchBad = oldClass.BranchCoverage < minBranchCoverage;
						bool oldClassSequenceBad = oldClass.SequenceCoverage < minSequenceCoverage;
						string oldClassBranchColor = oldClassBranchBad ? badColor : goodColor;
						string oldClassSequenceColor = oldClassSequenceBad ? badColor : goodColor;
						oldClassBranchCoverageMarkup = Markup.FromInterpolated($"[{oldClassBranchColor}] {oldClass.BranchCoverage}%[/]");
						oldClassCoverageMarkup = Markup.FromInterpolated($"[{oldClassSequenceColor}]{oldClass.SequenceCoverage}%[/]");
					}
				}

				var classNameText = Markup.FromInterpolated($"    [green]{@class.Name.Split('.').Last()}.cs[/]");
				var branchCoverageText = Markup.FromInterpolated($"[{classBranchColor}]{@class.BranchCoverage}%[/]");
				var coverageText = Markup.FromInterpolated($"[{classSequenceColor}]{@class.SequenceCoverage}%[/]");
				var classStatusMarkup = Markup.FromInterpolated($"[{classColor} bold]{classStatusText}[/]");
				assemblyTable.AddRow(classNameText, oldClassBranchCoverageMarkup, branchCoverageText, oldClassCoverageMarkup, coverageText, classStatusMarkup);

				foreach (var method in @class.Methods)
				{
					bool methodBranchBad = method.BranchCoverage < minBranchCoverage;
					bool methodSequenceBad = method.SequenceCoverage < minSequenceCoverage;
					bool methodIsBad = methodBranchBad || methodSequenceBad;
					string methodColor = methodIsBad ? badColor : goodColor;
					string methodBranchColor = methodBranchBad ? badColor : goodColor;
					string methodSequenceColor = methodSequenceBad ? badColor : goodColor;
					string methodStatusText = methodIsBad ? badText : goodText;

					var oldMethodBranchCoverageMarkup = Markup.FromInterpolated($"");
					var oldMethodCoverageMarkup = Markup.FromInterpolated($"");
					if (oldClass is not null)
					{
						var oldMethod = oldClass.Methods.FirstOrDefault(x => x.Name == method.Name);
						if (oldMethod is not null)
						{
							bool oldMethodBranchBad = oldMethod.BranchCoverage < minBranchCoverage;
							bool oldMethodSequenceBad = oldMethod.SequenceCoverage < minSequenceCoverage;
							string oldMethodBranchColor = oldMethodBranchBad ? badColor : goodColor;
							string oldMethodSequenceColor = oldMethodSequenceBad ? badColor : goodColor;
							oldMethodBranchCoverageMarkup = Markup.FromInterpolated($"[{oldMethodBranchColor}] {oldMethod.BranchCoverage}%[/]");
							oldMethodCoverageMarkup = Markup.FromInterpolated($"[{oldMethodSequenceColor}]{oldMethod.SequenceCoverage}%[/]");
						}
					}

					var methodNameMarkup = Markup.FromInterpolated($"      [magenta3_1]{method.Name.Split("::").Last()}[/]");
					var methodbranchCoverageMarkup = Markup.FromInterpolated($"[{methodBranchColor}]{method.BranchCoverage}%[/]");
					var methodcoverageMarkup = Markup.FromInterpolated($"[{methodSequenceColor}]{method.SequenceCoverage}%[/]");
					var methodstatusMarkup = Markup.FromInterpolated($"[{methodColor} bold]{methodStatusText}[/]");
					assemblyTable.AddRow(methodNameMarkup, oldMethodBranchCoverageMarkup, methodbranchCoverageMarkup, oldMethodCoverageMarkup, methodcoverageMarkup, methodstatusMarkup);
				}
			}

			assemblyTable.AddRow("");

		}

		AnsiConsole.WriteLine();
		AnsiConsole.Write(assemblyTable);
	}

	public static void BasycTestSaveToFile(CoverageReport coverageReport, string path)
	{
		var dto = CoverageReportJsonDto.ToDto(coverageReport);
		string json = JsonSerializer.Serialize(dto);
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		System.IO.File.CreateText(path).Dispose();
		System.IO.File.WriteAllText(path, json);
	}

	public static CoverageReport BasycTestLoadFromFile(string path)
	{
		string json = System.IO.File.ReadAllText(path);
		var dto = JsonSerializer.Deserialize<CoverageReportJsonDto>(json)!;
		var report = CoverageReportJsonDto.ToReport(dto);
		return report;
	}

	private static CoverageReport UnitTest(Solution solution, IEnumerable<string> projectToTestPaths, string textProjectSuffix = ".UnitTests")
	{
		var inProgressReport = new InProgressReport();
		inProgressReport.AddRange(solution, projectToTestPaths);

		var testProjectsToRun = inProgressReport.GetReportsToExecute();
		using var testSettingsFile = CreateRunSettings(testProjectsToRun.Select(x => x.ProjectToTestName));
		var projectResultDirectory = TemporaryDirectory.CreateNew($"{nameof(UnitTest)}/Projects");
		DotNetTest(_ => _
			.EnableNoRestore()
			.EnableNoBuild()
			.EnableCollectCoverage()
			.SetSettingsFile(testSettingsFile.FullPath)
			.CombineWith(testProjectsToRun,
		(settings, projectReport) => settings
				.SetResultsDirectory(projectResultDirectory.FullPath + "/" + projectReport.ProjectToTestName)
				.SetProjectFile(projectReport.TestProjectPath)),
			degreeOfParallelism: 5);

		Dictionary<string, TemporaryFile> projectToCoverageFileMap = new();
		foreach (var testProjectReport in testProjectsToRun)
		{
			var dir = new DirectoryInfo(projectResultDirectory.FullPath + "/" + testProjectReport.ProjectToTestName);
			var uniqueNameDir = dir.GetDirectories().First();

			string openCoverResultsFilePath = Path.Combine(uniqueNameDir.FullName, "coverage.opencover.xml");
			projectToCoverageFileMap.Add(testProjectReport.ProjectToTestName, TemporaryFile.CreateFromExisting(openCoverResultsFilePath));
			using var outputFileStream = System.IO.File.OpenRead(openCoverResultsFilePath);
			var openCoverCoverageSession = (CoverageSession)xmlSerializer.Deserialize(outputFileStream)!;
			var openCoverReport = openCoverCoverageSession.Modules.Module
				.Select(ParseOpencoverModule)
				.FirstOrDefault();

			if (openCoverReport == default)
			{
				//When parsing a open cover file returns 0 modules.
				//It means that there are 0 tests inside the test project
				//that is testing project to test.
				inProgressReport.Complete(testProjectReport.ProjectToTestName, new ProjectCoverageReport(testProjectReport.ProjectToTestName, true, false, 0, 0, Array.Empty<ClassCoverageReport>()));
			}
			else
			{
				inProgressReport.Complete(testProjectReport.ProjectToTestName, openCoverReport);
			}
		}

		//LogTestReport(inProgressReport);

		//for all bacthes etc. :
		//dotnet test /p:CollectCoverage=true /p:MergeWith='/path/to/result.json'
		var projectCoverageReports = inProgressReport.GetAllReports().Select(x => x.Report!).ToArray();
		return new CoverageReport(projectResultDirectory, projectCoverageReports, projectToCoverageFileMap);
	}

	private static void LogTestReport(InProgressReport inProgressReport)
	{
		Log.Debug("Coverage report:");
		var projectReports = inProgressReport.GetAllReports();
		foreach (var projectReport in projectReports)
		{
			Log.Debug($"		Assembly: {projectReport.ProjectToTestName} BranchCoverage: {projectReport!.Report!.BranchCoverage}% SequenceCoverage: {projectReport.Report.SequenceCoverage}% TestFound: {projectReport!.Report!.TestProjectFound} Excluded: {projectReport!.Report.CoverageExcluded}");

			foreach (var classReport in projectReport.Report.Classes)
			{
				Log.Debug($"			Class: {classReport.Name} BranchCoverage: {classReport.BranchCoverage}% SequenceCoverage: {classReport.SequenceCoverage}%");
				foreach (var methodReport in classReport.Methods)
				{
					Log.Debug($"				Method: {classReport.Name} BranchCoverage: {methodReport.BranchCoverage}% SequenceCoverage: {methodReport.SequenceCoverage}%");
				}
			}
		}
	}

	//Example and more options here:
	//https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
	private static TemporaryFile CreateRunSettings(IEnumerable<string> projectToTestNames)
	{
		string includeParam = string.Join(",", projectToTestNames.Select(x => $"[{x}]*"));

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
			          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute,ExcludeFromCodeCoverageAttribute</ExcludeByAttribute>
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

		var settingFile = TemporaryFile.CreateNewWith("coverlet", "runsettings", fileContent);
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
		double projectBranchCoverage = (double)Math.Round(module.Classes.Class
			.Select(x => double.Parse(x.Summary.BranchCoverage, CultureInfo.InvariantCulture.NumberFormat))
			.Average());
		double projectSequenceCoverage = (double)Math.Round(module.Classes.Class
			.Select(x => double.Parse(x.Summary.SequenceCoverage, CultureInfo.InvariantCulture.NumberFormat))
			.Average());

		return new ProjectCoverageReport(
				module.ModuleName,
				true,
				false,
				 projectBranchCoverage,
				 projectSequenceCoverage,
				 module.Classes.Class
				 .Select(classDto =>
				 {
					 string className = Path.GetFileNameWithoutExtension(classDto.FullName + ".cs");
					 double branchCoverage = ParseDouble(classDto.Summary.BranchCoverage);
					 double sequenceCoverage = ParseDouble(classDto.Summary.SequenceCoverage);

					 return new ClassCoverageReport(className, branchCoverage, sequenceCoverage, classDto.Methods.Method
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

