namespace Basyc.Localizator.Abstraction;

public class LocalizationStorageChangedEventArgs : EventArgs
{
    public LocalizationStorageChangedEventArgs(HashSet<string> sections)
    {
        ChangedSectionUniqueNames = sections;
    }

    public HashSet<string> ChangedSectionUniqueNames { get; }
}
