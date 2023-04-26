namespace Basyc.Localizator.Abstraction;

#pragma warning disable CA1002 // Do not expose generic lists

public class LocalizationManagerOptions
{
    public List<ILocalizator> LocalizatorsToSave { get; } = new();
}
