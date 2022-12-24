using FluentAssertions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction;

public class LocalizatedSection<TSection> : LocalizatedSection
{
	public LocalizatedSection(ILocalizatorStorage storage, IOptionsMonitor<LocalizationOptions> options, CultureInfo defaultCulture) : base(nameof(TSection), storage, options, defaultCulture = null)
	{

	}
}

public class LocalizatedSection : ILocalizatedSection
{
	protected IOptionsMonitor<LocalizationOptions> options;
	public string SectionUniqueName { get; private init; }
	private readonly bool contructorFinished;
	private CultureInfo defaultCulture;
	private readonly ILocalizatorStorage _storage;
	private bool inheritsDefaultCulture = true;
	private Dictionary<string, CultureInfo> supportedCultures { get; set; }
	public IReadOnlyDictionary<string, CultureInfo> SupportedCultures { get => supportedCultures; }
	public CultureInfo DefaultCulture
	{
		get => defaultCulture;
		set
		{
			if (SupportedCultures.ContainsKey(value.Name))
			{
				var oldCulture = defaultCulture;
				defaultCulture = value;
				OnSectionCultureChanged(oldCulture, value);
			}
			else
			{
				if (contructorFinished)
				{
					throw new Exception($"Can't change section default culture. Section with name {SectionUniqueName} does not support culture \"{value.Name}\"");
				}
				else
				{
					//Because initial default culture is not support by this section, default localizazor should return keys.
				}
			}
		}
	}

	public bool InheritsDefaultCulture
	{
		get => inheritsDefaultCulture;
		set
		{
			inheritsDefaultCulture = value;
			if (inheritsDefaultCulture == true && DefaultCulture != options.CurrentValue.SharedDefaultCulture)
			{
				DefaultCulture = options.CurrentValue.SharedDefaultCulture;
			}
		}
	}

	public LocalizatedSection(string sectionUniqueName, ILocalizatorStorage storage, IOptionsMonitor<LocalizationOptions> options, CultureInfo defaultCulture = null)
	{
		IOptionsMonitor<LocalizationOptions>
		_optionsMonitor = options;
		SectionUniqueName = sectionUniqueName;
		_storage = storage;
		DefaultCulture = defaultCulture ?? options.CurrentValue.SharedDefaultCulture;
		supportedCultures = new Dictionary<string, CultureInfo>();

	}

	public async Task<ILocalizator> GetLocalizatorAsync(CultureInfo culture)
	{
		GetLocalizatorResult getResult = await TryGetLocalizatorAsync(culture, false);
		ILocalizator localizator = getResult.localizator;

		if (getResult.localizatorFound == false)
		{
			if (InheritsDefaultCulture == true & DefaultCulture == culture)
			{
				localizator = new Localizator(culture, SectionUniqueName, new Dictionary<string, string>());
				localizator.CanGetReturnKey = true;
				localizator.CanGetReturnDefaultCultureValue = false;
			}
			else
			{
				throw new InvalidOperationException($"Section \"{SectionUniqueName}\" with culture {culture.Name} is not supported!");
			}
		}

		return new Localizator(localizator.Culture, localizator.SectionUniqueName, localizator.GetAll());
	}

	public Task<ILocalizator> GetLocalizatorAsync()
	{
		var culture = DefaultCulture;
		var defaultLocalizator = GetLocalizatorAsync(culture);
		return defaultLocalizator;
	}

	public async Task<GetLocalizatorResult> TryGetLocalizatorAsync(CultureInfo culture, bool canReturnDefault = true)
	{
		bool isCultureSupported;
		ILocalizator localizator = null;

		isCultureSupported = SupportedCultures.ContainsKey(culture.Name);

		if (isCultureSupported)
		{
			localizator = await _storage.LoadLocalizatorAsync(culture, SectionUniqueName);
			localizator = new Localizator(localizator.Culture, localizator.SectionUniqueName, localizator.GetAll());
		}
		else
		{
			if (canReturnDefault)
			{
				if (localizator == null || localizator.Culture.Name != DefaultCulture.Name)
				{
					localizator = await GetLocalizatorAsync();
				}
			}
		}

		return new GetLocalizatorResult(isCultureSupported, localizator);

	}

	public async Task AddLocalizatorsAsync(params ILocalizator[] localizators)
	{
		foreach (var localizator in localizators)
		{
			localizator.SectionUniqueName.Should().Be(SectionUniqueName, "Section name does not match");

			if (SupportedCultures.ContainsKey(localizator.Culture.Name) == false)
			{
				supportedCultures.Add(localizator.Culture.Name, localizator.Culture);
			}
		}

		await _storage.SaveOrUpdateLocalizatorsAsync(localizators);
	}

	public event EventHandler<SectionCultureChangedArgs> SectionCultureChanged;
	private void OnSectionCultureChanged(CultureInfo oldCulture, CultureInfo newCulture)
	{
		SectionCultureChanged?.Invoke(this, new SectionCultureChangedArgs(SectionUniqueName, oldCulture, newCulture));
	}
}
