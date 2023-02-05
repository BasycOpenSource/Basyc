namespace Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

public class StringNotGitFlowBranch : Exception
{
	public StringNotGitFlowBranch(string message) : base(message)
	{
	}
}
