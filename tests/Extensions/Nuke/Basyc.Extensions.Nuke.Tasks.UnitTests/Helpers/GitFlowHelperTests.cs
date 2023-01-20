using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;

namespace Basyc.Extensions.Nuke.Tasks.UnitTests.Helpers;
public class GitFlowHelperTests
{
	[Fact]
	public void IsPullRequestAllow_When_SourceBranchIsUnknown_Should_Throw()
	{
		var action = () => GitFlowHelper.IsPullRequestAllowed("UnknownBranch", "develop");
		action.Should().Throw<Exception>();
	}

	[Fact]
	public void IsPullRequestAllow_When_TargetBranchIsUnknown_ShouldNot_Throw()
	{
		var action = () => GitFlowHelper.IsPullRequestAllowed("develop", "UknownBranch");
		action.Should().NotThrow<Exception>();
	}
}
