using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Infrastructure.EF;

public interface ILocalizatorDbContext
{
    DbSet<LocalizatorSectionEntity> LocalizatedSections { get; }
    DbSet<LocalizatorEntity> Localizators { get; }
}
