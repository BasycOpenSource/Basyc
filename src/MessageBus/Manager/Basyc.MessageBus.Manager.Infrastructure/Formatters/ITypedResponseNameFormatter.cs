using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Application;

public interface ITypedResponseNameFormatter
{
    public string GetFormattedName(Type responseType);
}
