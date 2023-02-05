using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public class LocalizatedSection<TSection> : LocalizatedSection
{
	public LocalizatedSection(ILocalizatorStorage storage, IOptionsMonitor<LocalizationOptions> options, CultureInfo? defaultCulture) : base(nameof(TSection),
		storage, options, defaultCulture = null)
	{
	}
}

public class LocalizatedSection : ILocalizatedSection
{
	private readonly bool contructorFinished;
	protected readonly IOptionsMonitor<LocalizationOptions> options;
	private readonly ILocalizatorStorage storage;

	private readonly Dictionary<string, CultureInfo> supportedCultures;
	private CultureInfo defaultCulture;
	private bool inheritsDefaultCulture = true;

	public LocalizatedSection(string sectionUniqueName, ILocalizatorStorage storage, IOptionsMonitor<LocalizationOptions> options,
		CultureInfo? defaultCulture = null)
	{
		this.options = options;
		SectionUniqueName = sectionUniqueName;
		this.storage = storage;
		DefaultCulture = defaultCulture ?? options.CurrentValue.SharedDefaultCulture;
		this.defaultCulture = DefaultCulture;
		supportedCultures = new Dictionary<string, CultureInfo>();
	}

	public string SectionUniqueName { get; }
	public IReadOnlyDictionary<string, CultureInfo> SupportedCultures => supportedCultures;

	public CultureInfo DefaultCulture
	{
		get => defaultCulture;
		set
		{
			if (value is null)
			{
				throw new ArgumentException("cant be null");
			}

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
					throw new Exception(
						$"Can't change section default culture. Section with name {SectionUniqueName} does not support culture \"{value.Name}\"");
				}
				//Because initial default culture is not support by this section, default localizazor should return keys.
			}
		}
	}

	public bool InheritsDefaultCulture
	{
		get => inheritsDefaultCulture;
		set
		{
			inheritsDefaultCulture = value;
			if (inheritsDefaultCulture && DefaultCulture != options.CurrentValue.SharedDefaultCulture)
			{
				DefaultCulture = options.CurrentValue.SharedDefaultCulture;
			}
		}
	}

	public async Task<ILocalizator> GetLocalizatorAsync(CultureInfo culture)
	{
		var getResult = await TryGetLocalizatorAsync(culture, false);
		var localizator = getResult.Localizator;

		if (getResult.LocalizatorFound == false)
		{
			if (InheritsDefaultCulture & (DefaultCulture == culture))
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
		ILocalizator localizator;

		isCultureSupported = SupportedCultures.ContainsKey(culture.Name);

		if (isCultureSupported)
		{
			localizator = await storage.LoadLocalizatorAsync(culture, SectionUniqueName);
			localizator = new Localizator(localizator.Culture, localizator.SectionUniqueName, localizator.GetAll());
		}
		else
		{
			if (canReturnDefault)
			{
				localizator = await GetLocalizatorAsync();
			}
			else
			{
				throw new InvalidOperationException("Could not return localizator");
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

		await storage.SaveOrUpdateLocalizatorsAsync(localizators);
	}

	public event EventHandler<SectionCultureChangedArgs>? SectionCultureChanged;

	private void OnSectionCultureChanged(CultureInfo oldCulture, CultureInfo newCulture)
	{
		SectionCultureChanged?.Invoke(this, new SectionCultureChangedArgs(SectionUniqueName, oldCulture, newCulture));
	}
}
