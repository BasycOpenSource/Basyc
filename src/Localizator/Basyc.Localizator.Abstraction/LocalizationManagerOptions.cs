using System;
using System.Collections.Generic;
using System.Text;

namespace Basyc.Localizator.Abstraction;

public class LocalizationManagerOptions
{
    public List<ILocalizator> LocalizatorsToSave { get; set; }
}
