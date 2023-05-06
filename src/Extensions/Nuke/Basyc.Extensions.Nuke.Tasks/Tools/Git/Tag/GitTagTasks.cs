using Nuke.Common.Git;
using Nuke.Common.Tooling;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public static partial class GitTasks
{
    public static void GitCreateTag(string tagName, GitRepository gitRepository)
    {
        global::Nuke.Common.Tools.Git.GitTasks.Git("config --global user.email \"bot@dummyemail.com\"");
        global::Nuke.Common.Tools.Git.GitTasks.Git("config --global user.name \"Automated Bot\"");
        global::Nuke.Common.Tools.Git.GitTasks.Git($"tag -a {tagName} -m \"Creating git tag '{tagName}'\"");
        global::Nuke.Common.Tools.Git.GitTasks.Git("push --tags", customLogger: Logger);
    }

    private static void Logger(OutputType type, string s)
    {
        if (type is OutputType.Err)
        {
            if (s.StartsWith("To http") || s.StartsWith(" * [new tag]"))
            {
                global::Nuke.Common.Tools.Git.GitTasks.GitLogger.Invoke(OutputType.Std, s);
                return;
            }
        }

        global::Nuke.Common.Tools.Git.GitTasks.GitLogger.Invoke(type, s);
    }
}
