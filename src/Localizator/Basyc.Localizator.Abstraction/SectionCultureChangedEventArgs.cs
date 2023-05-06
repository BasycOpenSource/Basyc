using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public class SectionCultureChangedEventArgs : EventArgs
{
    public SectionCultureChangedEventArgs(string sectionName, CultureInfo oldCulture, CultureInfo newCulture)
    {
        SectionName = sectionName;
        OldCulture = oldCulture;
        NewCulture = newCulture;
    }

    public string SectionName { get; }

    public CultureInfo OldCulture { get; }

    public CultureInfo NewCulture { get; }
}
