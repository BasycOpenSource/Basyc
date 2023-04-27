using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Basyc.Localizator.Abstraction;

public interface ILocalizator : IStringLocalizer
{
    event EventHandler<LocalizatorValuesChangedEventArgs> ValuesChanged;

    CultureInfo Culture { get; }

    string SectionUniqueName { get; }

    bool CanGetReturnDefaultCultureValue { get; set; }

    bool CanGetReturnKey { get; set; }

    /// <summary>
    /// Returns localized value, exception if not found.
    /// </summary>
    new string this[string key] { get; }

    IDictionary<string, string> GetAll();

    /// <summary>
    /// Returns localized value, exception if not found.
    /// </summary>
    string Get(string key);

    /// <summary>
    /// If values is not found the default value is returned.
    /// </summary>
    bool TryGet(string key, out string value);

    Task EditValues(IDictionary<string, string> newValues);
}
