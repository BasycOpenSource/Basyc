namespace Basyc.Extensions.Nuke.Targets;

public class NugetSettings
{
	public string? SourceUrl { get; private set; }
	public string? SourceApiKey { get; private set; }

	public static NugetSettings Create()
	{
		return new NugetSettings();
	}

	public NugetSettings SetSourceUrl(string sourceUrl)
	{
		SourceUrl = sourceUrl;
		return this;
	}

	public NugetSettings SetSourceApiKey(string sourceApiKey)
	{
		SourceApiKey = sourceApiKey;
		return this;
	}
}
