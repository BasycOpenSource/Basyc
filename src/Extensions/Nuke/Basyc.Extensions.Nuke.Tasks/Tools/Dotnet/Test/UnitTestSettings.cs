namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
public class UnitTestSettings
{
	public static UnitTestSettings Create()
	{
		return new UnitTestSettings();
	}

	public List<ProjectTestException> ProjectExceptions { get; init; } = new();
	public double BranchMinimum { get; private set; } = 50;
	public double SequenceMinimum { get; private set; } = 50;
	public string UnitTestSuffix { get; private set; } = ".UnitTests";

	public UnitTestSettings Exclude(string projectPath)
	{
		ProjectExceptions.Add(new ProjectTestException(projectPath));
		return this;
	}

	public UnitTestSettings SetBranchMinimum(double branchMinimum)
	{
		BranchMinimum = branchMinimum;
		return this;
	}

	public UnitTestSettings SetSequenceMinimum(double sequenceMinimum)
	{
		SequenceMinimum = sequenceMinimum;
		return this;
	}

	public UnitTestSettings SetUnitTestSuffix(string testSuffix)
	{
		UnitTestSuffix = testSuffix;
		return this;
	}
}
