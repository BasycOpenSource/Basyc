using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public interface ILocalizationManager
{
    event EventHandler<SectionCultureChangedEventArgs> SectionCultureChanged;

    CultureInfo DefaultCulture { get; set; }

    IDictionary<string, ILocalizatedSection> GetSections();

    ILocalizatedSection GetSection(string sectionName);

    /// <summary>
    ///     Returns false if section does not exist.
    /// </summary>
    bool TryGetSection(string sectionName, [NotNullWhen(true)] out ILocalizatedSection? localizatedSection);

    Task SaveOrUpdateLocalizators(params ILocalizator[] localizators);

    void ChangeDefaultSectionCulture(string sectionName, CultureInfo newCulture);
}
