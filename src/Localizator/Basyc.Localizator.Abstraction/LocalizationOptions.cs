using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public class LocalizationOptions
{
	public LocalizationOptions(CultureInfo sharedDefaultCulture)
	{
		SharedDefaultCulture = sharedDefaultCulture;
	}

	/// <summary>
	///     Initial culture when no other value is specified
	/// </summary>
	public CultureInfo SharedDefaultCulture { get; init; }

	/// <summary>
	///     Overriding <see cref="SharedDefaultCulture" />. String key is <see cref="ILocalizatedSection.SectionUniqueName" />
	/// </summary>
	public Dictionary<string, CultureInfo> SectionsDefaultCultures { get; } = new();
}
