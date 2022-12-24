using System;
using System.Collections.Generic;
using System.Text;

namespace Basyc.Localizator.Abstraction;

public class LocalizationStorageChangedArgs
{
	public HashSet<string> ChangedSectionUniqueNames { get; }
	public LocalizationStorageChangedArgs(HashSet<string> sections)
	{
		ChangedSectionUniqueNames = sections;
	}
}
