using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction
{
    public class LocalizatorValuesChangedArgs
    {
        public LocalizatorValuesChangedArgs(IDictionary<string, string> newValues)
        {
            NewValues = newValues;
        }

        public IDictionary<string, string> NewValues { get; }
    }
}