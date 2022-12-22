using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Basyc.Localizator.Abstraction;

public class LocalizationOptions
{
	public LocalizationOptions()
	{

	}
	/// <summary>
	/// Initial culture when no other value is specified
	/// </summary>
	public CultureInfo SharedDefaultCulture { get; set; }
	/// <summary>
	/// Overriding <see cref="SharedDefaultCulture"/>. String key is <see cref="ILocalizatedSection.SectionUniqueName"/>
	/// </summary>
	public Dictionary<string, CultureInfo> SectionsDefaultCultures { get; set; }
}