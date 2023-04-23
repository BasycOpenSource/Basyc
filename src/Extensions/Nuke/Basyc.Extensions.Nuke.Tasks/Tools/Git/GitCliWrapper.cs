using System.Text;
using CliWrap;

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

        string outputStr = stdOutBuffer.ToString();
        string errorStr = stdErrBuffer.ToString();
        if (errorStr != string.Empty)
            throw new InvalidOperationException(errorStr);

        string[] credentials = outputStr.Split('\n');
        string userName = credentials[0].Split('=')[1];
        string password = credentials[1].Split('=')[1];
        return new(userName, password);
    }
}
