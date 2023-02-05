using CliWrap;
using System.Text;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static class GitCliWrapper
{
	public static async Task<GitCredentials> GetCredentialsWindows()
	{
		var stdOutBuffer = new StringBuilder();
		var stdErrBuffer = new StringBuilder();
		var result = await Cli.Wrap(global::Nuke.Common.Tools.Git.GitTasks.GitPath)
			.WithArguments("credential-wincred get")
			.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
			.WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
			.WithStandardInputPipe(PipeSource.FromString("protocol=https\nhost=github.com\n\n"))
			.ExecuteAsync();

		var outputStr = stdOutBuffer.ToString();
		var errorStr = stdErrBuffer.ToString();
		if (errorStr != "")
			throw new Exception(errorStr);

		var credentials = outputStr.Split('\n');
		var userName = credentials[0].Split('=')[1];
		var password = credentials[1].Split('=')[1];
		return new GitCredentials(userName, password);
	}
}
