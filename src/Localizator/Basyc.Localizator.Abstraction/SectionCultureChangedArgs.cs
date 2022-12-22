using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Basyc.Localizator.Abstraction;

public class SectionCultureChangedArgs : EventArgs
{
    public SectionCultureChangedArgs(string sectionName, CultureInfo oldCulture, CultureInfo newCulture)
    {
        SectionName = sectionName;
        OldCulture = oldCulture;
        NewCulture = newCulture;
    }

    public string SectionName { get; set; }
    public CultureInfo OldCulture { get; set; }
    public CultureInfo NewCulture { get; set; }
}
