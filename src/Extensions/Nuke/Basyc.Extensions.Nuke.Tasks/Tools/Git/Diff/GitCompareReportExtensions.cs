namespace Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
public static class GitCompareReportExtensions
{
	public static void ThrowIfNotValid(this GitCompareReport report)
	{
		if (report is null || report.CouldCompare is false)
		{
			throw new InvalidOperationException("Git compare report is not valid");
		}
	}
}
