using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public interface ILocalizatedSection<T> : ILocalizatedSection
{
}

public interface ILocalizatedSection
{
    event EventHandler<SectionCultureChangedEventArgs>? SectionCultureChanged;

    /// <summary>
    ///     Name of localization group.
    /// </summary>
    string SectionUniqueName { get; }

    CultureInfo DefaultCulture { get; set; }

    /// <summary>
    ///     Default culture will be always same as it is in LocalizationManager.
    /// </summary>
    bool InheritsDefaultCulture { get; set; }

    /// <summary>
    ///     <see cref="string" /> key is <see cref="CultureInfo.Name" />.
    /// </summary>
    IReadOnlyDictionary<string, CultureInfo> SupportedCultures { get; }

    /// <summary>
    ///     Return localizazor for provided culture.
    /// </summary>
    Task<ILocalizator> GetLocalizatorAsync(CultureInfo culture);

    /// <summary>
    ///     Returns localizazor with default localization culture or default culture of this section when section's culture is setuped.
    /// </summary>
    Task<ILocalizator> GetLocalizatorAsync();

    Task<GetLocalizatorResult> TryGetLocalizatorAsync(CultureInfo culture, bool canReturnDefault = true);

    /// <summary>
    ///     Saves multiple <paramref name="localizators" />.
    /// </summary>
    Task AddLocalizatorsAsync(params ILocalizator[] localizators);
}
