namespace Basyc.Localizator.Abstraction;

public class LocalizationStorageChangedArgs
{
    public LocalizationStorageChangedArgs(HashSet<string> sections)
    {
        ChangedSectionUniqueNames = sections;
    }

    public HashSet<string> ChangedSectionUniqueNames { get; }
}
