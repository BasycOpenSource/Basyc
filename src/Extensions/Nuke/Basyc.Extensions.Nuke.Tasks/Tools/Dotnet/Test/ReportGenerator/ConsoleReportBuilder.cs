using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Palmmedia.ReportGenerator.Core.Reporting;
using Spectre.Console;
using System.Globalization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.ReportGenerator;
public class ConsoleReportBuilder : IReportBuilder
{
	public ConsoleReportBuilder()
	{
	}
	public static string ReportTypeName => "BasycConsole";
	public string ReportType => ReportTypeName;

	public IReportContext? ReportContext { get; set; }

	public void CreateClassReport(Class @class, IEnumerable<FileAnalysis> fileAnalyses)
	{

	}

	public void CreateSummaryReport(SummaryResult summaryResult)
	{
		if (ReportContext is null)
		{
			throw new ArgumentNullException(nameof(ReportContext));
		}

		//Console.WriteLine("ConsoleReportBuilder CreateSummaryReport");

		//string targetPath = Path.Combine(ReportContext.ReportConfiguration.TargetDirectory, ReportOutputShortFileName);
		//using var reportTextWriter = new StreamWriter(new FileStream(targetPath, FileMode.Create), Encoding.UTF8);

		foreach (var assembly in summaryResult.Assemblies)
		{
			//Console.WriteLine(
			//	"{0};{1}",
			//	assembly.Name,
			//	assembly.CoverageQuota.HasValue ? assembly.CoverageQuota.Value.ToString("f1", CultureInfo.InvariantCulture) + "%" : string.Empty);

			//string coverageText = assembly.CoverageQuota.HasValue ? assembly.CoverageQuota.Value.ToString("f1", CultureInfo.InvariantCulture) + "%" : string.Empty;
			//AnsiConsole.MarkupLineInterpolated($"[red]{nameof(assembly.Name)}[/] {coverageText}");

			AnsiConsole.WriteLine();
			AnsiConsole.MarkupInterpolated($"[bold]{assembly.Name}.csproj[/]");
			AnsiConsole.Write($" ");
			if (assembly.CoverageQuota.HasValue)
			{
				AnsiConsole.Write(assembly.CoverageQuota.Value.ToString("f1", CultureInfo.InvariantCulture) + "%");
			}
			else
			{
				AnsiConsole.Write("none");
			}

			//if (assembly.Classes.Any())
			//{
			//	Console.WriteLine();
			//}

			//foreach (var @class in assembly.Classes)
			//{
			//	Console.WriteLine(
			//		"{0};{1}",
			//		@class.Name,
			//		@class.CoverageQuota.HasValue ? @class.CoverageQuota.Value.ToString("f1", CultureInfo.InvariantCulture) + "%" : string.Empty);
			//}

			var assemblyTable = new Table();
			assemblyTable.AddColumn("class");
			assemblyTable.AddColumn(new TableColumn("b. cov.").RightAligned());
			assemblyTable.AddColumn(new TableColumn("s. cov.").RightAligned());
			assemblyTable.AddColumn(new TableColumn("status").Centered());

			foreach (var @class in assembly.Classes)
			{
				var classNameText = Markup.FromInterpolated($"[green3]{@class.Name.Split('.').Last()}.cs[/]");
				var branchCoverageText = new Text($"{@class.BranchCoverageQuota}%");
				var coverageText = new Text($"{@class.CoverageQuota}%");
				assemblyTable.AddRow(classNameText, branchCoverageText, coverageText);
			}

			AnsiConsole.WriteLine();
			AnsiConsole.Write(assemblyTable);
		}
	}
}
