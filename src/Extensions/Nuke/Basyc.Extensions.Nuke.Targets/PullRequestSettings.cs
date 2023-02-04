namespace Basyc.Extensions.Nuke.Targets;

public class PullRequestSettings
{
	public bool IsPullRequest { get; private set; }
	public string? SourceBranch { get; private set; }
	public string? TargetBranch { get; private set; }

	public static PullRequestSettings Create()
	{
		return new PullRequestSettings();
	}

	public PullRequestSettings SetIsPullRequest(bool isPullRequest)
	{
		IsPullRequest = isPullRequest;
		return this;
	}

	public PullRequestSettings SetSourceBranch(string? sourceBranch)
	{
		SourceBranch = sourceBranch;
		return this;
	}

	public PullRequestSettings SetTargetBranch(string? targetBranch)
	{
		TargetBranch = targetBranch;
		return this;
	}

	public void AsserIfValid()
	{
		if (IsPullRequest is false)
			return;

		if (string.IsNullOrEmpty(SourceBranch))
			throw new Exception($"{nameof(SourceBranch)} can't be empty when {nameof(IsPullRequest)} is set to true");

		if (string.IsNullOrEmpty(TargetBranch))
			throw new Exception($"{nameof(TargetBranch)} can't be empty when {nameof(IsPullRequest)} is set to true");
	}
}
