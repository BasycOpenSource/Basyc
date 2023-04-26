using System.Globalization;

namespace Basyc.Localizator.Abstraction;

/// <summary>
/// Contains and manages storage for all localization sections.
/// </summary>
public interface ILocalizatorStorage
{
    event EventHandler<LocalizationStorageChangedEventArgs> StorageChanged;

    Task<IDictionary<string, ILocalizatedSection>> GetSectionsAsync();

    Task<ILocalizatedSection> GetSectionAsync(string uniqueSectionName);

    /// <summary>
    /// <see cref="string"/> key is <see cref="CultureInfo.Name"/>.
    /// </summary>
    Task<IDictionary<string, CultureInfo>> GetSupportedCulturesAsync(string sectionUniqueName);

    Task<ILocalizator> LoadLocalizatorAsync(CultureInfo requiredCulture, string sectionUniqueName);

    Task SaveOrUpdateLocalizatorsAsync(params ILocalizator[] localizators);
}
