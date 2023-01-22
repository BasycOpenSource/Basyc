using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Palmmedia.ReportGenerator.Core;
using Spectre.Console;
using System.Text;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{
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
			var oldProject = PrintProject(minSequenceCoverage, minBranchCoverage, oldCoverageReport, assemblyTable, project);

			foreach (var @class in project.Classes)
			{
				var oldClass = PrintClass(minSequenceCoverage, minBranchCoverage, assemblyTable, oldProject, @class);

				foreach (var method in @class.Methods)
				{
					PrintMethod(minSequenceCoverage, minBranchCoverage, assemblyTable, oldClass, method);
				}
			}
		}

		AnsiConsole.WriteLine();
		AnsiConsole.Write(assemblyTable);
	}

	private static void PrintMethod(double minSequenceCoverage, double minBranchCoverage, Table assemblyTable, ClassCoverageReport? oldClass, MethodCoverageReport method)
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

		string friendlyMethodName = GetFriendlyMethodDescription(method.Name);
		var methodNameMarkup = Markup.FromInterpolated($"      [magenta3_1]{friendlyMethodName}[/]");
		var methodBranchCoverageMarkup = GetPercetangeMarkup(method.BranchCoverage, oldMethodBranchCoverage, minBranchCoverage);
		var methodSequenceCoverageMarkup = GetPercetangeMarkup(method.SequenceCoverage, oldMethodSequenceCoverage, minSequenceCoverage);
		var methodstatusMarkup = Markup.FromInterpolated($"[{methodColor} bold]{methodStatusText}[/]");
		assemblyTable.AddRow(methodstatusMarkup, methodNameMarkup, methodBranchCoverageMarkup, methodSequenceCoverageMarkup);
	}

	private static string GetFriendlyMethodDescription(string methodDescription)
	{
		int methodNameIndex = methodDescription.IndexOf("::");
		var methodDesc = methodDescription.AsSpan(methodNameIndex + 2);
		int paramsIndex = methodDesc.IndexOf('(');
		var methodName = methodDesc.Slice(0, paramsIndex);

		var stringBuilder = new StringBuilder();
		stringBuilder.Append(methodName);
		var parametersLine = methodDesc.Slice(paramsIndex);
		if (parametersLine.Length is 2)
		{
			stringBuilder.Append('(');
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		stringBuilder.Append('(');
		var parameters = parametersLine.Split(',');
		foreach (char[] parameter in parameters)
		{
			int lastDotIndex = Array.LastIndexOf(parameter, '.');
			if (lastDotIndex is -1)
			{
				stringBuilder.Append(parameter);
			}
			else
			{
				var parameterFriendlyName = parameter.AsSpan(lastDotIndex + 1, parameter.Length - lastDotIndex - 2);
				stringBuilder.Append(parameterFriendlyName);
			}

			stringBuilder.Append(", ");
		}

		stringBuilder.Remove(stringBuilder.Length - 2, 2);

		stringBuilder.Append(')');
		return stringBuilder.ToString();
	}

	private static ProjectCoverageReport? PrintProject(double minSequenceCoverage, double minBranchCoverage, CoverageReport? oldCoverageReport, Table assemblyTable, ProjectCoverageReport project)
	{
		bool projectBranchBad = project.BranchCoverage < minBranchCoverage;
		bool projectSequenceBad = project.SequenceCoverage < minSequenceCoverage;
		bool projectIsBad = project.CoverageExcluded is false && (projectBranchBad || projectSequenceBad);
		string projectColor = projectIsBad ? newBadColor : newGoodColor;
		string projectBranchColor = projectBranchBad ? newBadColor : newGoodColor;
		string projectSequenceColor = projectSequenceBad ? newBadColor : newGoodColor;
		string projectstatusText = projectIsBad ? badText : goodText;
		string projectNameTag = GetProjectTag(project);

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

		if (project.CoverageExcluded)
		{
			projectBranchCoverageText = Markup.FromInterpolated($"--");
			projectSequenceCoverageText = Markup.FromInterpolated($"--");
		}

		assemblyTable.AddRow(projectStatusMarkup, projectNameText, projectBranchCoverageText, projectSequenceCoverageText);
		return oldProject;
	}

	private static string GetProjectTag(ProjectCoverageReport project)
	{
		string projectNameTag = "";
		if (project.CoverageExcluded)
			projectNameTag = "(excluded)";
		else
			projectNameTag = project.TestProjectFound ? "" : $"[{newBadColor}](tests missing)[/]";
		return projectNameTag;
	}

	private static ClassCoverageReport? PrintClass(double minSequenceCoverage, double minBranchCoverage, Table assemblyTable, ProjectCoverageReport? oldProject, ClassCoverageReport @class)
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
		return oldClass;
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

		return Markup.FromInterpolated($"([{changeColor}]{valueDiffText}%[/]) [{color}]{newPercentage,4}%[/]");
	}
}

