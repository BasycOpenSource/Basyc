using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Throw;

namespace Basyc.Localizator.Abstraction;

public class LocalizationManager : ILocalizationManager
{
	private readonly LocalizationOptions localizationOptions;
	private readonly IOptions<LocalizationManagerOptions> managerOptions;
	private readonly Dictionary<string, ILocalizatedSection> sections;
	private readonly ILocalizatorStorage storage;
	private CultureInfo defaultCulture;

	public LocalizationManager(IOptions<LocalizationOptions> localizationOptions, IOptions<LocalizationManagerOptions> managerOptions,
		ILocalizatorStorage storage)
	{
		this.storage = storage;
		this.localizationOptions = localizationOptions.Value;
		this.managerOptions = managerOptions;
		DefaultCulture = this.localizationOptions.SharedDefaultCulture;
		defaultCulture = DefaultCulture;
		sections = new Dictionary<string, ILocalizatedSection>();
		foreach (var storageSection in storage.GetSectionsAsync().GetAwaiter().GetResult().Values)
		{
			sections.Add(storageSection.SectionUniqueName, storageSection);
			storageSection.SectionCultureChanged += SectionCultureChangedHandler;
		}

		this.storage.StorageChanged += (s, a) =>
		{
			foreach (var changedSectionName in a.ChangedSectionUniqueNames)
			{
				var oldSectionExists = sections.TryGetValue(changedSectionName, out var oldSection);

				if (oldSectionExists)
				{
					//oldSection.SectionCultureChanged -= SectionCultureChangedHandler;
					//sections.Remove(oldSection.SectionUniqueName);
				}
				else
				{
					var newSection = this.storage.GetSectionAsync(changedSectionName).GetAwaiter().GetResult();
					newSection.SectionCultureChanged += SectionCultureChangedHandler;
					sections.Add(newSection.SectionUniqueName, newSection);
				}
			}
		};

		SaveOrUpdateLocalizators(this.managerOptions.Value.LocalizatorsToSave.ToArray());
	}

	public CultureInfo DefaultCulture
	{
		get => defaultCulture;
		set
		{
			defaultCulture = value;
			//_localizationOptions.DefaultCulture = value;
			foreach (var sectionPair in sections)
			{
				var section = sectionPair.Value;
				if (section.InheritsDefaultCulture)
				{
					section.DefaultCulture = value;
				}
			}
		}
	}

	public event EventHandler<SectionCultureChangedArgs>? SectionCultureChanged;

	public IDictionary<string, ILocalizatedSection> GetSections()
	{
		return sections;
	}

	public ILocalizatedSection GetSection(string sectionName)
	{
		if (TryGetSection(sectionName, out var section) == false)
		{
			throw new Exception($"Section with key \"{sectionName}\" does not exist");
		}

		return section;
	}

	public bool TryGetSection(string sectionName, [NotNullWhen(true)] out ILocalizatedSection? localizatedSection)
	{
		if (string.IsNullOrWhiteSpace(sectionName))
		{
			throw new ArgumentNullException(nameof(sectionName));
		}

		if (sections.TryGetValue(sectionName, out localizatedSection))
		{
			return true;
		}

		localizatedSection = null;
		return false;
	}

	public Task SaveOrUpdateLocalizators(params ILocalizator[] localizators)
	{
		return storage.SaveOrUpdateLocalizatorsAsync(localizators);
	}

	public void ChangeDefaultSectionCulture(string sectionName, CultureInfo newCulture)
	{
		if (string.IsNullOrWhiteSpace(sectionName))
		{
			throw new ArgumentNullException(nameof(sectionName));
		}

		if (sections.TryGetValue(sectionName, out var section))
		{
			var oldCulture = section.DefaultCulture;
			section.DefaultCulture = newCulture;
		}
		else
		{
			throw new Exception($"Can't change section default culture. Section with name {sectionName} does not exists");
		}
	}

	private void SectionCultureChangedHandler(object? sender, SectionCultureChangedArgs a)
	{
		sender.ThrowIfNull();
		var section = (ILocalizatedSection)sender;
		OnSectionCultureChanged(section.SectionUniqueName, a.OldCulture, a.NewCulture);
	}

	private void OnSectionCultureChanged(string sectionName, CultureInfo oldCulture, CultureInfo newCulture)
	{
		SectionCultureChanged?.Invoke(this, new SectionCultureChangedArgs(sectionName, oldCulture, newCulture));
	}
}
