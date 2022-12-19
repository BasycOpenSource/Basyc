using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction
{
    public class LocalizationManager : ILocalizationManager
    {
        private ILocalizatorStorage _storage;
        private LocalizationOptions _localizationOptions;
        private IOptions<LocalizationManagerOptions> _managerOptions;
        private Dictionary<string, ILocalizatedSection> sections;

        CultureInfo defaultCulture;
        public CultureInfo DefaultCulture
        {
            get => defaultCulture;
            set
            {
                defaultCulture = value;
                //_localizationOptions.DefaultCulture = value;
                foreach (KeyValuePair<string, ILocalizatedSection> sectionPair in sections)
                {
                    ILocalizatedSection section = sectionPair.Value;
                    if (section.InheritsDefaultCulture == true)
                    {
                        section.DefaultCulture = value;
                    }
                }

            }

        }

        public event EventHandler<SectionCultureChangedArgs> SectionCultureChanged;


        public LocalizationManager(IOptions<LocalizationOptions> localizationOptions, IOptions<LocalizationManagerOptions> managerOptions, ILocalizatorStorage storage)
        {
            _storage = storage;
            _localizationOptions = localizationOptions.Value;
            _managerOptions = managerOptions;
            DefaultCulture = _localizationOptions.SharedDefaultCulture;

            sections = new Dictionary<string, ILocalizatedSection>();
            foreach (var storageSection in storage.GetSectionsAsync().GetAwaiter().GetResult().Values)
            {
                sections.Add(storageSection.SectionUniqueName, storageSection);
                storageSection.SectionCultureChanged += SectionCultureChangedHandler;
            }

            _storage.StorageChanged += (s, a) =>
            {
                foreach (string changedSectionName in a.ChangedSectionUniqueNames)
                {
                    bool oldSectionExists = sections.TryGetValue(changedSectionName, out ILocalizatedSection oldSection);

                    if (oldSectionExists)
                    {
                        //oldSection.SectionCultureChanged -= SectionCultureChangedHandler;
                        //sections.Remove(oldSection.SectionUniqueName);
                    }
                    else
                    {
                        var newSection = _storage.GetSectionAsync(changedSectionName).GetAwaiter().GetResult();
                        newSection.SectionCultureChanged += SectionCultureChangedHandler;
                        sections.Add(newSection.SectionUniqueName, newSection);
                    }
                }
            };

            SaveOrUpdateLocalizators(_managerOptions.Value.LocalizatorsToSave.ToArray());
        }

        private void SectionCultureChangedHandler(object sender, SectionCultureChangedArgs a)
        {
            var section = (ILocalizatedSection)sender;
            OnSectionCultureChanged(section.SectionUniqueName, a.OldCulture, a.NewCulture);
        }

        public IDictionary<string, ILocalizatedSection> GetSections()
        {
            return sections;
        }

        public ILocalizatedSection GetSection(string sectionName)
        {
            var sectionFound = TryGetSection(sectionName, out var section);
            if (sectionFound == false)
            {
                throw new Exception($"Section with key \"{sectionName}\" does not exist");
            }

            return section;

        }

        public bool TryGetSection(string sectionName, out ILocalizatedSection localizatedSection)
        {

            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new ArgumentNullException(nameof(sectionName));
            }

            var sectionExists = sections.TryGetValue(sectionName, out localizatedSection);
            if (sectionExists)
            {

                return true;
            }
            else
            {
                localizatedSection = null;
                return false;
            }
        }

        public Task SaveOrUpdateLocalizators(params ILocalizator[] localizators)
        {
            return _storage.SaveOrUpdateLocalizatorsAsync(localizators);
        }

        public void ChangeDefaultSectionCulture(string sectionName, CultureInfo newCulture)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new ArgumentNullException(nameof(sectionName));
            }

            var oldSectionExists = sections.TryGetValue(sectionName, out ILocalizatedSection section);
            if (oldSectionExists)
            {
                CultureInfo oldCulture = section.DefaultCulture;
                section.DefaultCulture = newCulture;

            }
            else
            {
                throw new Exception($"Can't change section default culture. Section with name {sectionName} does not exists");
            }

        }

        private void OnSectionCultureChanged(string sectionName, CultureInfo oldCulture, CultureInfo newCulture)
        {
            SectionCultureChanged?.Invoke(this, new SectionCultureChangedArgs(sectionName, oldCulture, newCulture));
        }


    }
}
