using System.ComponentModel.DataAnnotations;

namespace Basyc.Localizator.Infrastructure.EF;
#pragma warning disable CA1002 // Do not expose generic lists

public class LocalizatorSectionEntity
{
    public LocalizatorSectionEntity(string uniqueSectionName)
    {
        UniqueSectionName = uniqueSectionName;
    }

    [Key]
    public string UniqueSectionName { get; init; }

    public List<LocalizatorEntity> Localizators { get; } = new();
}
