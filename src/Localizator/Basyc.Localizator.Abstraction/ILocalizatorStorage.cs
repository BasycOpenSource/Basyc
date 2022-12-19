using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction
{
    /// <summary>
    /// Contains and manages storage for all localization sections
    /// </summary>
    public interface ILocalizatorStorage
    {
        Task<IDictionary<string, ILocalizatedSection>> GetSectionsAsync();
        Task<ILocalizatedSection> GetSectionAsync(string uniqueSectionName);
        /// <summary>
        /// <see cref="string"/> key is <see cref="CultureInfo.Name"/>
        /// </summary>
        /// <param name="sectionUniqueName"></param>
        /// <returns></returns>
        Task<IDictionary<string, CultureInfo>> GetSupportedCulturesAsync(string sectionUniqueName);

        Task<ILocalizator> LoadLocalizatorAsync(CultureInfo requiredCulture, string sectionUniqueName);

        Task SaveOrUpdateLocalizatorsAsync(params ILocalizator[] localizators);

        event EventHandler<LocalizationStorageChangedArgs> StorageChanged;


    }
}