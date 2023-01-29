using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Format;
using Newtonsoft.Json;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet;

public static class DotnetWrapper
{
	public static bool FormatVerifyNoChanges(string workingDirectory, string project, IEnumerable<string> filesToCheck, out DotnetFormatReport report,
		out ProcessException? processException)
	{
		filesToCheck = filesToCheck.Select(x => Path.GetRelativePath(Path.GetDirectoryName(project)!, x).Replace('\\', '/'));

		string formatReportFilePath = Path.GetTempPath() + $"dotnetFormatReport-{Random.Shared.Next()}.json";

		bool isFormatted;
		try
		{
			string includeParam = string.Join(' ', filesToCheck);
			string include = filesToCheck.Any() ? $" --include {includeParam}" : "";
			DotNet($"format \"{project}\"{include} --verify-no-changes --no-restore --report \"{formatReportFilePath}\" --verbosity quiet",
				logOutput: false,
				workingDirectory: workingDirectory);

			isFormatted = true;
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

			isFormatted = false;
			processException = ex;
			string fileContent = File.ReadAllText(formatReportFilePath);
			report = new DotnetFormatReport(JsonConvert.DeserializeObject<ReportRecord[]>(fileContent));
		}

		File.Delete(formatReportFilePath);
		return isFormatted;
	}

	public static void NugetSignWithFile(IEnumerable<string> packagesPaths, string certPath, string? certPassword)
	{
		DotNet(
			$"nuget sign {string.Join(' ', packagesPaths)} --certificate-path {certPath} --certificate-password {certPassword} --timestamper http://timestamp.digicert.com  --overwrite",
			logOutput: false);
	}

	public static void NugetSignWithBase64(IEnumerable<string> packagesPaths, string base64Cert, string? certPassword)
	{
		byte[] certContent = Convert.FromBase64String(base64Cert);
		using var cert = TemporaryFile.CreateNewWith(fileExtension: "pfx", content: certContent);
		DotNet(
			$"nuget sign {string.Join(' ', packagesPaths)} --certificate-path {cert.FullPath} --certificate-password {certPassword} --timestamper http://timestamp.digicert.com  --overwrite",
			logOutput: false);
	}
}
