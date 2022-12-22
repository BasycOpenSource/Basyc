using Basyc.Localizator.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Basyc.Localizator.Infrastructure.EF;

public class EFLocalizatorStorage : ILocalizatorStorage
{
    private readonly DbContext dbContext;
    private readonly IOptionsMonitor<LocalizationOptions> options;

    public EFLocalizatorStorage(DbContext dbContext, IOptionsMonitor<LocalizationOptions> options)
    {
        TestDbContext(dbContext);
        this.dbContext = dbContext;
        this.options = options;
    }
    public event EventHandler<LocalizationStorageChangedArgs> StorageChanged;
    private void OnStorageChanged(HashSet<string> sections)
    {
        StorageChanged?.Invoke(this, new LocalizationStorageChangedArgs(sections));
    }

    public async Task<IDictionary<string, ILocalizatedSection>> GetSectionsAsync()
    {
        var sections = await dbContext.Set<LocalizatorSectionEntity>().ToDictionaryAsync(x => x.UniqueSectionName, x => (ILocalizatedSection)SectionToModel(x));
        return sections;
    }

    public async Task<IDictionary<string, CultureInfo>> GetSupportedCulturesAsync(string sectionUniqueName)
    {
        var cultures = await dbContext.Set<LocalizatorSectionEntity>().AsQueryable().Where(x => x.UniqueSectionName == sectionUniqueName).Select(x => x.UniqueSectionName).ToDictionaryAsync(x => x, x => new CultureInfo(x));
        return cultures;
    }

    public async Task<ILocalizator> LoadLocalizatorAsync(CultureInfo requiredCulture, string sectionUniqueName)
    {
        var localizator = await dbContext.Set<LocalizatorEntity>().FindAsync(sectionUniqueName, requiredCulture.Name);
        return LocalizatorToModel(localizator);
    }

    public async Task SaveOrUpdateLocalizatorsAsync(params ILocalizator[] localizators)
    {

        HashSet<string> sections = new HashSet<string>();
        foreach (var localizator in localizators)
        {
            sections.Add(localizator.SectionUniqueName);
            dbContext.Set<LocalizatorEntity>().Update(LocalizatorToEntity(localizator));
        }

        await dbContext.SaveChangesAsync();
        OnStorageChanged(sections);
    }

    private LocalizatedSection SectionToModel(LocalizatorSectionEntity entity)
    {
        var model = new LocalizatedSection(entity.UniqueSectionName, this, options);
        return model;
    }

    private ILocalizator LocalizatorToModel(LocalizatorEntity entity)
    {
        var model = new Basyc.Localizator.Abstraction.Localizator(new CultureInfo(entity.CultureName), entity.SectionUniqueName, entity.Values);
        return model;
    }

    private LocalizatorEntity LocalizatorToEntity(ILocalizator model)
    {
        var entity = new LocalizatorEntity(model.Culture.Name, model.SectionUniqueName, new Dictionary<string, string>(model.GetAll()), new LocalizatorSectionEntity() { UniqueSectionName = model.SectionUniqueName });
        return entity;
    }

    /// <summary>
    /// Ensures all entities exists in <paramref name="dbContext"/>
    /// </summary>
    /// <param name="dbContext"></param>
    private static void TestDbContext(DbContext dbContext)
    {
        dbContext.Set<LocalizatorSectionEntity>();
        dbContext.Set<LocalizatorEntity>();
    }

    public async Task<ILocalizatedSection> GetSectionAsync(string uniqueSectionName)
    {
        var section = await dbContext.Set<LocalizatorSectionEntity>().FindAsync(uniqueSectionName);
        return SectionToModel(section);
    }
}
