using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction;

public interface ILocalizator : IStringLocalizer
{

	CultureInfo Culture { get; }
	string SectionUniqueName { get; }
	bool CanGetReturnDefaultCultureValue { get; set; }
	bool CanGetReturnKey { get; set; }

	IDictionary<string, string> GetAll();
	/// <summary>
	/// Returns localized value, exception if not found
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	string Get(string key);
	/// <summary>
	/// If values is not found the default value is returned
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	bool TryGet(string key, out string value);
	/// <summary>
	/// Returns localized value, exception if not found
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	new string this[string key] { get; }

	event EventHandler<LocalizatorValuesChangedArgs> ValuesChanged;
	Task EditValues(IDictionary<string, string> newValues);

}