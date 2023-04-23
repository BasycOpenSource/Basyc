using System.Globalization;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Palmmedia.ReportGenerator.Core.Reporting;
using Spectre.Console;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.ReportGenerator;

public class ConsoleReportBuilder : IReportBuilder
{
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
            ArgumentNullException.ThrowIfNull(ReportContext);
        }

        foreach (var assembly in summaryResult.Assemblies)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupInterpolated($"[bold]{assembly.Name}.csproj[/]");
            AnsiConsole.Write(" ");
            if (assembly.CoverageQuota.HasValue)
            {
                AnsiConsole.Write(assembly.CoverageQuota.Value.ToString("f1", CultureInfo.InvariantCulture) + "%");
            }
            else
            {
                AnsiConsole.Write("none");
            }

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
