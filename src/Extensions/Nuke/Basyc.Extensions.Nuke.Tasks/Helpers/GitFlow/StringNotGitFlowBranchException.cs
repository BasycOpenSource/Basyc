namespace Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

public class StringNotGitFlowBranchException : Exception
{
    public StringNotGitFlowBranchException(string message) : base(message)
    {
    }
}
