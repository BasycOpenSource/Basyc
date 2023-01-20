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

	private const string goodText = "ok";
	private const string oldGoodColor = "darkgreen";
	private const string newGoodColor = "green1";
	private const string badText = "bad";
	private const string oldBadColor = "darkred_1";
	private const string newBadColor = "red1";
	public static void BasycTestCreateSummaryConsole(CoverageReport coverageReport, double minSequenceCoverage, double minBranchCoverage, CoverageReport? oldCoverageReport = null)
	{

		var assemblyTable = new Table();
		assemblyTable.AddColumn(new TableColumn("ok?").Centered());
		assemblyTable.AddColumn("file");
		assemblyTable.AddColumn(new TableColumn("branch").RightAligned());
		assemblyTable.AddColumn(new TableColumn("line").RightAligned());

		foreach (var project in coverageReport.Projects)
		{

			bool projectBranchBad = project.BranchCoverage < minBranchCoverage;
			bool projectSequenceBad = project.SequenceCoverage < minSequenceCoverage;
			bool projectIsBad = projectBranchBad || projectSequenceBad;
			string projectColor = projectIsBad ? newBadColor : newGoodColor;
			string projectBranchColor = projectBranchBad ? newBadColor : newGoodColor;
			string projectSequenceColor = projectSequenceBad ? newBadColor : newGoodColor;
			string projectstatusText = projectIsBad ? badText : goodText;
			string projectNameTag = project.TestProjectFound ? "" : $"[{newBadColor}](test not found)[/]";

			ProjectCoverageReport? oldProject = null;
			double? oldProjectBranchCoverage = null;
			double? oldProjectSequenceCoverage = null;
			if (oldCoverageReport is not null)
			{
				oldProject = oldCoverageReport.Projects.FirstOrDefault(x => x.Name == project.Name);
				if (oldProject is not null)
				{
					oldProjectBranchCoverage = oldProject.BranchCoverage;
					oldProjectSequenceCoverage = oldProject.SequenceCoverage;
				}
			}

			var projectNameText = new Markup($"[bold]{project.Name}.csproj[/] {projectNameTag}");
			var projectBranchCoverageText = GetPercetangeMarkup(project.BranchCoverage, oldProjectBranchCoverage, minBranchCoverage);
			var projectSequenceCoverageText = GetPercetangeMarkup(project.SequenceCoverage, oldProjectSequenceCoverage, minSequenceCoverage);
			var projectStatusMarkup = Markup.FromInterpolated($"[{projectColor} bold]{projectstatusText}[/]");

			assemblyTable.AddRow(projectStatusMarkup, projectNameText, projectBranchCoverageText, projectSequenceCoverageText);

			foreach (var @class in project.Classes)
			{
				bool classBranchBad = @class.BranchCoverage < minBranchCoverage;
				bool classSequenceBad = @class.SequenceCoverage < minSequenceCoverage;
				bool classIsBad = classBranchBad || classSequenceBad;
				string classColor = classIsBad ? newBadColor : newGoodColor;
				string classBranchColor = classBranchBad ? newBadColor : newGoodColor;
				string classSequenceColor = classSequenceBad ? newBadColor : newGoodColor;
				string classStatusText = classIsBad ? badText : goodText;

				ClassCoverageReport? oldClass = null;
				double? oldClassBranchCoverage = null;
				double? oldClassSequenceCoverage = null;
				if (oldProject is not null)
				{
					oldClass = oldProject.Classes.FirstOrDefault(x => x.Name == @class.Name);
					if (oldClass is not null)
					{
						oldClassBranchCoverage = oldClass.BranchCoverage;
						oldClassSequenceCoverage = oldClass.SequenceCoverage;
					}
				}

				var classNameText = Markup.FromInterpolated($"    [green]{@class.Name.Split('.').Last()}.cs[/]");
				var branchCoverageText = GetPercetangeMarkup(@class.BranchCoverage, oldClassBranchCoverage, minBranchCoverage);
				var sequenceCoverageText = GetPercetangeMarkup(@class.SequenceCoverage, oldClassSequenceCoverage, minSequenceCoverage);

				var classStatusMarkup = Markup.FromInterpolated($"[{classColor} bold]{classStatusText}[/]");
				assemblyTable.AddRow(classStatusMarkup, classNameText, branchCoverageText, sequenceCoverageText);

				foreach (var method in @class.Methods)
				{
					bool methodBranchBad = method.BranchCoverage < minBranchCoverage;
					bool methodSequenceBad = method.SequenceCoverage < minSequenceCoverage;
					bool methodIsBad = methodBranchBad || methodSequenceBad;
					string methodColor = methodIsBad ? newBadColor : newGoodColor;
					string methodBranchColor = methodBranchBad ? newBadColor : newGoodColor;
					string methodSequenceColor = methodSequenceBad ? newBadColor : newGoodColor;
					string methodStatusText = methodIsBad ? badText : goodText;

					double? oldMethodBranchCoverage = null;
					double? oldMethodSequenceCoverage = null;
					if (oldClass is not null)
					{
						var oldMethod = oldClass.Methods.FirstOrDefault(x => x.Name == method.Name);
						if (oldMethod is not null)
						{
							oldMethodBranchCoverage = oldMethod.BranchCoverage;
							oldMethodSequenceCoverage = oldMethod.SequenceCoverage;
						}
					}

					var methodNameMarkup = Markup.FromInterpolated($"      [magenta3_1]{method.Name.Split("::").Last()}[/]");
					var methodBranchCoverageMarkup = GetPercetangeMarkup(method.BranchCoverage, oldMethodBranchCoverage, minBranchCoverage);
					var methodSequenceCoverageMarkup = GetPercetangeMarkup(method.SequenceCoverage, oldMethodSequenceCoverage, minSequenceCoverage);
					var methodstatusMarkup = Markup.FromInterpolated($"[{methodColor} bold]{methodStatusText}[/]");
					assemblyTable.AddRow(methodstatusMarkup, methodNameMarkup, methodBranchCoverageMarkup, methodSequenceCoverageMarkup);
				}
			}

			assemblyTable.AddRow("");

		}

		AnsiConsole.WriteLine();
		AnsiConsole.Write(assemblyTable);
	}

	private static Markup GetPercetangeMarkup(double newPercentage, double? oldPercentage, double minimum)
	{
		newPercentage = Math.Round(newPercentage, 0);
		if (oldPercentage.HasValue)
			oldPercentage = Math.Round(oldPercentage.Value, 0);

		bool valueBad = newPercentage < minimum;
		string color = valueBad ? newBadColor : newGoodColor;

		if (oldPercentage is null || newPercentage == oldPercentage)
			return Markup.FromInterpolated($"[{color}]{newPercentage}%[/]");

		bool valueIncreased = newPercentage >= oldPercentage;
		double valueDiff = Math.Round(newPercentage - oldPercentage.Value, 0);
		string valueDiffText = (valueDiff > 0 ? "+" : "") + valueDiff;
		string changeColor = valueIncreased ? oldGoodColor : oldBadColor;

		return Markup.FromInterpolated($"[{color}]{newPercentage}%[/]([{changeColor}]{valueDiffText}%[/])");
	}

	public static void BasycCoverageSaveToFile(CoverageReport coverageReport, string path)
	{
		var dto = CoverageReportJsonDto.ToDto(coverageReport);
		string json = JsonSerializer.Serialize(dto);
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		System.IO.File.CreateText(path).Dispose();
		System.IO.File.WriteAllText(path, json);
	}

	public static CoverageReport BasycCoverageLoadFromFile(string path)
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

