using System.Globalization;
using Basyc.Localizator.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Throw;

namespace Basyc.Localizator.Infrastructure.EF;

public class EfLocalizatorStorage : ILocalizatorStorage
{
    private readonly DbContext dbContext;

    private readonly IOptionsMonitor<LocalizationOptions> options;

    public EfLocalizatorStorage(DbContext dbContext, IOptionsMonitor<LocalizationOptions> options)
    {
        TestDbContext(dbContext);
        this.dbContext = dbContext;
        this.options = options;
    }

    public event EventHandler<LocalizationStorageChangedEventArgs>? StorageChanged;

    public async Task<IDictionary<string, ILocalizatedSection>> GetSectionsAsync()
    {
        var sections = await dbContext.Set<LocalizatorSectionEntity>().ToDictionaryAsync(x => x.UniqueSectionName, x => (ILocalizatedSection)SectionToModel(x));
        return sections;
    }

    public async Task<IDictionary<string, CultureInfo>> GetSupportedCulturesAsync(string sectionUniqueName)
    {
        var cultures = await dbContext.Set<LocalizatorSectionEntity>().AsQueryable().Where(x => x.UniqueSectionName == sectionUniqueName)
            .Select(x => x.UniqueSectionName).ToDictionaryAsync(x => x, x => new CultureInfo(x));
        return cultures;
    }

    public async Task<ILocalizator> LoadLocalizatorAsync(CultureInfo requiredCulture, string sectionUniqueName)
    {
        var localizator = await dbContext.Set<LocalizatorEntity>().FindAsync(sectionUniqueName, requiredCulture.Name);
        localizator.ThrowIfNull();
        return LocalizatorToModel(localizator);
    }

    public async Task SaveOrUpdateLocalizatorsAsync(params ILocalizator[] localizators)
    {
        var sections = new HashSet<string>();
        foreach (var localizator in localizators)
        {
            sections.Add(localizator.SectionUniqueName);
            dbContext.Set<LocalizatorEntity>().Update(LocalizatorToEntity(localizator));
        }

        await dbContext.SaveChangesAsync();
        OnStorageChanged(sections);
    }

    public async Task<ILocalizatedSection> GetSectionAsync(string uniqueSectionName)
    {
        var section = await dbContext.Set<LocalizatorSectionEntity>().FindAsync(uniqueSectionName);
        section.ThrowIfNull();
        return SectionToModel(section);
    }

    private static ILocalizator LocalizatorToModel(LocalizatorEntity entity)
    {
        var model = new Abstraction.Localizator(new(entity.CultureName), entity.SectionUniqueName, entity.Values);
        return model;
    }

    private static LocalizatorEntity LocalizatorToEntity(ILocalizator model)
    {
        var entity = new LocalizatorEntity(model.Culture.Name,
            model.SectionUniqueName,
            new(model.GetAll()),
            new(model.SectionUniqueName));
        return entity;
    }

    /// <summary>
    ///     Ensures all entities exists in <paramref name="dbContext" />.
    /// </summary>
    private static void TestDbContext(DbContext dbContext)
    {
        dbContext.Set<LocalizatorSectionEntity>();
        dbContext.Set<LocalizatorEntity>();
    }

    private void OnStorageChanged(HashSet<string> sections) => StorageChanged?.Invoke(this, new(sections));

    private LocalizatedSection SectionToModel(LocalizatorSectionEntity entity)
    {
        var model = new LocalizatedSection(entity.UniqueSectionName, this, options);
        return model;
    }
}
