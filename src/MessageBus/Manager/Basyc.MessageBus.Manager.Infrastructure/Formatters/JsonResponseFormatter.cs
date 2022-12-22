using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure.Formatters;

public class JsonResponseFormatter : IResponseFormatter
{
    public string Format(object response)
    {
        return JsonSerializer.Serialize(response);
    }
}
