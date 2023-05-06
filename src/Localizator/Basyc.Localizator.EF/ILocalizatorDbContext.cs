using Microsoft.EntityFrameworkCore;

namespace Basyc.Localizator.Infrastructure.EF;

public interface ILocalizatorDbContext
{
    DbSet<LocalizatorSectionEntity> LocalizatedSections { get; }

    DbSet<LocalizatorEntity> Localizators { get; }
}
