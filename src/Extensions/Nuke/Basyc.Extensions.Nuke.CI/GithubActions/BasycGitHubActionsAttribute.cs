using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Utilities;

namespace Basyc.Extensions.Nuke.CI.GithubActions;

#pragma warning disable CA1813 // Avoid unsealed attributes
public class BasycGitHubActionsAttribute : GitHubActionsAttribute
{
    public BasycGitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images) : base(name, image, images)
    {
        Name = name;
        Image = image;
        Images = images;
    }

    public string[] ImportParameters { get; set; } = Array.Empty<string>();

    public string Name { get; }

    public GitHubActionsImage Image { get; }

    public GitHubActionsImage[] Images { get; }

    protected override IEnumerable<(string Key, string Value)> GetImports()
    {
        foreach (var valueTuple in base.GetImports())
            yield return valueTuple;

        foreach (string param in ImportParameters)
            yield return (param, GetParameterValue(param));
    }

    private static string GetParameterValue(string parameter) =>
        $"${{{{ vars.{parameter.SplitCamelHumpsWithKnownWords().JoinUnderscore().ToUpperInvariant()} }}}}";
}
