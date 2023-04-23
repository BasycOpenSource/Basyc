namespace Basyc.Localizator.Abstraction;

public class LocalizationManagerOptions
{
    public List<ILocalizator> LocalizatorsToSave { get; set; } = new();
}
