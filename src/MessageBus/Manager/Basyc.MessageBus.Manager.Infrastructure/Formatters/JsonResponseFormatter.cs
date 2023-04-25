using System.Text.Json;

namespace Basyc.MessageBus.Manager.Infrastructure.Formatters;

public class JsonResponseFormatter : IResponseFormatter
{
    public string Format(object response) => JsonSerializer.Serialize(response);
}
