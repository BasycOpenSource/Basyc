using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public interface ILocalizatedSection<T> : ILocalizatedSection
{
}

public interface ILocalizatedSection
{
	/// <summary>
	///     Name of localization group
	/// </summary>
	string SectionUniqueName { get; }

	CultureInfo DefaultCulture { get; set; }

	/// <summary>
	///     Default culture will be always same as it is in LocalizationManager.
	/// </summary>
	bool InheritsDefaultCulture { get; set; }

	/// <summary>
	/// Checks if this section supports localization for provided culture
	/// </summary>
	/// <param name="cultureInfo"></param>
	/// <returns></returns>

	//Task<bool> IsCultureSupportedAsync(CultureInfo cultureInfo);
	/// <summary>
	///     <see cref="string" /> key is <see cref="CultureInfo.Name" />
	/// </summary>
	IReadOnlyDictionary<string, CultureInfo> SupportedCultures { get; }

	/// <summary>
	///     Return localizazor for provided culture
	/// </summary>
	/// <param name="culture"></param>
	/// <returns></returns>
	Task<ILocalizator> GetLocalizatorAsync(CultureInfo culture);

	/// <summary>
	///     Returns localizazor with default localization culture or default culture of this section when section's culture is setuped
	/// </summary>
	Task<ILocalizator> GetLocalizatorAsync();

	/// <param name="culture"></param>
	/// <param name="localizator"></param>
	/// <param name="canReturnDefault"></param>
	/// <returns></returns>
	Task<GetLocalizatorResult> TryGetLocalizatorAsync(CultureInfo culture, bool canReturnDefault = true);

	/// <summary>
	///     Saves multiple <paramref name="localizators" />
	/// </summary>
	/// <param name="localizators"></param>
	/// <returns></returns>
	Task AddLocalizatorsAsync(params ILocalizator[] localizators);

	event EventHandler<SectionCultureChangedArgs>? SectionCultureChanged;
}
