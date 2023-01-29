using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public interface ILocalizationManager
{
	CultureInfo DefaultCulture { get; set; }

	IDictionary<string, ILocalizatedSection> GetSections();
	ILocalizatedSection GetSection(string sectionName);

	/// <summary>
	///     Returns false if section does not exist.
	/// </summary>
	/// <param name="sectionName"></param>
	/// <returns></returns>
	bool TryGetSection(string sectionName, [NotNullWhen(true)] out ILocalizatedSection? localizatedSection);

	Task SaveOrUpdateLocalizators(params ILocalizator[] localizators);

	event EventHandler<SectionCultureChangedArgs> SectionCultureChanged;

	//void ChangeDefaultCulture(CultureInfo newCulture);
	void ChangeDefaultSectionCulture(string sectionName, CultureInfo newCulture);
}
