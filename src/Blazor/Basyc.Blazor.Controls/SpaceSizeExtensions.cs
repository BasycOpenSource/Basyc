namespace Basyc.Blazor.Controls;

public static class SpaceSizeExtensions
{
    public static string GetSizeCssVariable(this SpaceSize size) => size switch
    {
        SpaceSize.None => "0px",
        SpaceSize.Smaller => "var(--space-smaller)",
        SpaceSize.Small => "var(--space-small)",
        SpaceSize.Medium => "var(--space-medium)",
        SpaceSize.Big => "var(--space-big)",
        SpaceSize.Bigger => "var(--space-bigger)",
        _ => throw new ArgumentException("not expected enum value", nameof(size))
    };
}
