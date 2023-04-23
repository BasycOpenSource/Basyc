using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Utilities;

namespace Basyc.Extensions.Nuke.CI.GithubActions;

public class BasycGitHubActionsAttribute : GitHubActionsAttribute
{
    public BasycGitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images) : base(name, image, images)
    {
    }

    public string[] ImportParameters { get; set; } = Array.Empty<string>();

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
