using Microsoft.Extensions.Localization;
using System.Globalization;
using Throw;

namespace Basyc.Localizator.Abstraction;

public class Localizator : ILocalizator
{
    private IDictionary<string, string> values;

    public Localizator(CultureInfo culture, string sectionName, IDictionary<string, string> values) : this(culture, sectionName, values, null)
    {
    }

    public Localizator(CultureInfo culture, string sectionName, IDictionary<string, string> values, ILocalizator? backupLocalizator)
    {
        BackupLocalizator = backupLocalizator;
        this.values = values;
        Culture = culture;
        SectionUniqueName = sectionName;
    }

    public event EventHandler<LocalizatorValuesChangedArgs>? ValuesChanged;

    public ILocalizator? BackupLocalizator { get; }

    public CultureInfo Culture { get; }

    public string SectionUniqueName { get; }

    public bool CanGetReturnDefaultCultureValue { get; set; }

    public bool CanGetReturnKey { get; set; }

    public string this[string key] => Get(key);

    public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

    LocalizedString IStringLocalizer.this[string name]
    {
        get
        {
            bool wasLocalized = TryGet(name, out string? localizedValue);
            var localString = new LocalizedString(name, localizedValue, !wasLocalized);
            return localString;
        }
    }

    public string Get(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (values.TryGetValue(key, out string? value))
        {
            return value;
        }

        if (CanGetReturnDefaultCultureValue)
        {
            if (BackupLocalizator != null)
            {
                if (BackupLocalizator.TryGet(key, out value))
                {
                    return value;
                }

                if (CanGetReturnKey)
                {
                    return key;
                }

                throw new InvalidOperationException(
                    $"Localizer of section {SectionUniqueName} could not find value for {key} in culture {Culture.Name} and default culture {BackupLocalizator.Culture}. Try add localized value or set property {nameof(CanGetReturnKey)} to true");
            }

            if (CanGetReturnKey)
            {
                return key;
            }

            throw new InvalidOperationException(
                $"Localizer of section {SectionUniqueName} could not find value for {key} in culture {Culture.Name}. {nameof(CanGetReturnDefaultCultureValue)} is set to true but default localizer is null. Try add localized value or set property {nameof(CanGetReturnKey)} to true");
        }

        if (CanGetReturnKey)
        {
            return key;
        }

        throw new InvalidOperationException(
            $"Localizer of section {SectionUniqueName} could not find value for {key} in culture {Culture.Name}. Try add localized value or set property {nameof(CanGetReturnDefaultCultureValue)} or {nameof(CanGetReturnKey)} to true");
    }

    public bool TryGet(string key, out string value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (values.TryGetValue(key, out string? valueFromValues))
        {
            valueFromValues.ThrowIfNull();
            value = valueFromValues;
            return true;
        }

        if (BackupLocalizator is not null && BackupLocalizator.TryGet(key, out value))
        {
            return true;
        }

        value = key;
        return false;
    }

    public IDictionary<string, string> GetAll() => values;

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var localizedStrings = new List<LocalizedString>();
        var allKeys = GetAll().Keys;
        foreach (string key in allKeys)
        {
            bool wasLocalized = TryGet(key, out string? localizedValue);
            var localizedString = new LocalizedString(key, localizedValue, !wasLocalized);
            localizedStrings.Add(localizedString);
        }

        return localizedStrings;
    }

    public Task EditValues(IDictionary<string, string> newValues)
    {
        values = newValues;
        OnValuesChanged(newValues);

        return Task.CompletedTask;
    }

    private void OnValuesChanged(IDictionary<string, string> newValues) => ValuesChanged?.Invoke(this, new LocalizatorValuesChangedArgs(newValues));
}
