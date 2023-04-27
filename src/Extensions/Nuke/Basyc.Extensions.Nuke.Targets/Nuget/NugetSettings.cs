namespace Basyc.Extensions.Nuke.Targets;

public class NugetSettings
{
    public Uri? SourceUrl { get; private set; }

    public string? SourceApiKey { get; private set; }

    public static NugetSettings Create() => new();

    public NugetSettings SetSourceUrl(Uri sourceUrl)
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
