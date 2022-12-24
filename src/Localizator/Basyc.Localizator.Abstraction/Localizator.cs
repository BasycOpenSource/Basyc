using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction;

public class Localizator : ILocalizator
{

	private IDictionary<string, string> _values;

	public CultureInfo Culture { get; private set; }
	public string SectionUniqueName { get; private set; }
	public ILocalizator _backupLocalizator { get; private set; }
	public bool CanGetReturnDefaultCultureValue { get; set; }
	public bool CanGetReturnKey { get; set; }

	public string this[string key]
	{
		get
		{
			return Get(key);
		}
	}

	public Localizator(CultureInfo culture, string sectionName, IDictionary<string, string> values) : this(culture, sectionName, values, null)
	{

	}

	public Localizator(CultureInfo culture, string sectionName, IDictionary<string, string> values, ILocalizator backupLocalizator)
	{
		_backupLocalizator = backupLocalizator;
		_values = values;
		Culture = culture;
		SectionUniqueName = sectionName;
	}

	public event EventHandler<LocalizatorValuesChangedArgs> ValuesChanged;
	private void OnValuesChanged(IDictionary<string, string> newValues)
	{
		ValuesChanged?.Invoke(this, new LocalizatorValuesChangedArgs(newValues));
	}

	public string Get(string key)
	{
		ArgumentNullException.ThrowIfNull(key);
		if (_values.TryGetValue(key, out string value))
			return value;

		if (CanGetReturnDefaultCultureValue)
		{
			if (_backupLocalizator != null)
			{
				if (_backupLocalizator.TryGet(key, out value))
					return value;

				if (CanGetReturnKey)
					return key;

				throw new Exception($"Localizer of section {SectionUniqueName} could not find value for {key} in culture {Culture.Name} and default culture {_backupLocalizator.Culture}. Try add localized value or set property {nameof(CanGetReturnKey)} to true");

			}

			if (CanGetReturnKey)
				return key;

			throw new Exception($"Localizer of section {SectionUniqueName} could not find value for {key} in culture {Culture.Name}. {nameof(CanGetReturnDefaultCultureValue)} is set to true but default localizer is null. Try add localized value or set property {nameof(CanGetReturnKey)} to true");

		}

		if (CanGetReturnKey)
			return key;

		throw new Exception($"Localizer of section {SectionUniqueName} could not find value for {key} in culture {Culture.Name}. Try add localized value or set property {nameof(CanGetReturnDefaultCultureValue)} or {nameof(CanGetReturnKey)} to true");

	}

	public bool TryGet(string key, out string value)
	{
		ArgumentNullException.ThrowIfNull(key);

		if (_values.TryGetValue(key, out value))
			return true;

		if (_backupLocalizator is not null && _backupLocalizator.TryGet(key, out value))
			return true;

		value = key;
		return false;
	}

	public IDictionary<string, string> GetAll()
	{
		return _values;
	}

	//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
	//IStringLocalizer implementation
	//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
	public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

	LocalizedString IStringLocalizer.this[string name]
	{
		get
		{
			var wasLocalized = TryGet(name, out string localizedValue);
			LocalizedString localString = new LocalizedString(name, localizedValue, !wasLocalized);
			return localString;
		}
	}

	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		List<LocalizedString> localizedStrings = new List<LocalizedString>();
		var allKeys = GetAll().Keys;
		foreach (var key in allKeys)
		{
			var wasLocalized = TryGet(key, out string localizedValue);
			LocalizedString localizedString = new LocalizedString(key, localizedValue, !wasLocalized);
			localizedStrings.Add(localizedString);
		}

		return localizedStrings;

	}

	public Task EditValues(IDictionary<string, string> newValues)
	{
		_values = newValues;
		OnValuesChanged(newValues);

		return Task.CompletedTask;
	}
}
