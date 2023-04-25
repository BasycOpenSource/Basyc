namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public class UnitTestSettings
{
    public List<ProjectTestExceptionPath> ProjectExceptions { get; init; } = new();

    public double BranchMinimum { get; private set; } = 50;

    public double SequenceMinimum { get; private set; } = 50;

    public string UnitTestSuffix { get; private set; } = ".UnitTests";

    public bool PublishResults { get; private set; }

    public static UnitTestSettings Create() => new();

    public UnitTestSettings Exclude(string projectPath)
    {
        ProjectExceptions.Add(new(projectPath));
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

    public UnitTestSettings SetPublishResults(bool publishResults)
    {
        PublishResults = publishResults;
        return this;
    }
}
