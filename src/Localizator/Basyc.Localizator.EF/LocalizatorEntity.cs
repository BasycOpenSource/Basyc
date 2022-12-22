using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Infrastructure.EF;

public class LocalizatorEntity
{
    public LocalizatorEntity(string cultureName, string sectionUniqueName, Dictionary<string, string> values, LocalizatorSectionEntity section)
    {
        CultureName = cultureName;
        SectionUniqueName = sectionUniqueName;
        Values = values;
        Section = section;
    }

    public string CultureName { get; set; }
    public string SectionUniqueName { get; set; }
    public Dictionary<string, string> Values { get; set; }
    public LocalizatorSectionEntity Section { get; set; }
}
