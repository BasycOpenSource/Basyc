using Newtonsoft.Json;
using Nuke.Common.Tooling;
using System.Diagnostics.CodeAnalysis;
using Tasks.Dotnet.Format;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Tasks.Dotnet;

public static class DotnetWrapper
{

	public static bool FormatVerifyNoChanges(string workingDirectory, string project, IEnumerable<string> filesTocheck, out DotnetFormatReport report, out ProcessException? processException)
	{
		filesTocheck = filesTocheck.Select(x => Path.GetRelativePath(Path.GetDirectoryName(project)!, x).Replace('\\', '/'));

		var formatReportFilePath = Path.GetTempPath() + $"dotnetFormatReport-{Random.Shared.Next()}.json";

		bool isFormated;
		try
		{
			var includeParam = string.Join(' ', filesTocheck);
			var include = filesTocheck.Any() ? $" --include {includeParam}" : "";
			DotNet($"format \"{project}\"{include} --verify-no-changes --no-restore --report \"{formatReportFilePath}\" --verbosity quiet",
			logOutput: false,
			workingDirectory: workingDirectory);

			isFormated = true;
			processException = null;
			report = new DotnetFormatReport(Array.Empty<ReportRecord>());
		}
		catch (ProcessException ex)
		{
			if (ex.Message.StartsWith("Process 'dotnet.exe' exited with code 2.") is false)
			{
				File.Delete(formatReportFilePath);
				throw;
			}

			isFormated = false;
			processException = ex;
			var fileContent = File.ReadAllText(formatReportFilePath);
			report = new(JsonConvert.DeserializeObject<ReportRecord[]>(fileContent));
		}

		File.Delete(formatReportFilePath);
		return isFormated;
	}
}