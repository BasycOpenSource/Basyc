using Newtonsoft.Json;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks.Dotnet.Format;

public static class DotnetWrapper
{

	public static bool FormatVerifyNoChanges(string workingDirectory, string project, IEnumerable<string> filesTocheck, out DotnetFormatReport report, out ProcessException? processException)
	{
		filesTocheck = filesTocheck.Select(x => Path.GetRelativePath(Path.GetDirectoryName(project)!, x).Replace('\\', '/'));

		string formatReportFilePath = Path.GetTempPath() + $"dotnetFormatReport-{Random.Shared.Next()}.json";

		bool isFormated;
		try
		{
			string includeParam = string.Join(' ', filesTocheck);
			string include = filesTocheck.Any() ? $" --include {includeParam}" : "";
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
			string fileContent = File.ReadAllText(formatReportFilePath);
			report = new(JsonConvert.DeserializeObject<ReportRecord[]>(fileContent));
		}

		File.Delete(formatReportFilePath);
		return isFormated;
	}

	public static void Test(string projectDll, string collect)
	{
		//DotNet($"test \"{projectDll}\" --collect:\"{collect}\"");
		//DotNet($"test --collect:\"{collect}\"", workingDirectory: Directory.GetParent(projectDll).FullName);
		DotNet($"test --collect:\"{collect}\" --no-build", workingDirectory: @"C:\Users\Honza\source\repos\BasycOpenSource\Basyc\tests\Serialization\Basyc.Serialization.ProtobufNet.UnitTests");
		//DotNet($"test \"{projectDll}\" /p:CollectCoverage=true");
		//DotNet($"test /p:CollectCoverage=true");

	}
}
