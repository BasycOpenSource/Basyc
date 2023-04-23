using System.ComponentModel.DataAnnotations;

namespace Basyc.Localizator.Infrastructure.EF;

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
