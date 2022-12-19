using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction
{
    public interface ILocalizationManager
    {
        CultureInfo DefaultCulture { get; set; }

        IDictionary<string, ILocalizatedSection> GetSections();
        ILocalizatedSection GetSection(string sectionName);
        /// <summary>
        /// Returns false if section does not exist.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        bool TryGetSection(string sectionName, out ILocalizatedSection localizatedSection);
        Task SaveOrUpdateLocalizators(params ILocalizator[] localizators);
        event EventHandler<SectionCultureChangedArgs> SectionCultureChanged;
        //void ChangeDefaultCulture(CultureInfo newCulture);
        void ChangeDefaultSectionCulture(string sectionName, CultureInfo newCulture);
    }
}
