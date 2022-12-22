using Basyc.MessageBus.Manager.Application;
using System.Text;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers;

public static class ResponseResultConverter
{
    public static string CreateInputOverview(IEnumerable<Parameter> parameters)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var parameter in parameters)
        {
            var paramterValueString = parameter.Value is null ? "null" : parameter.Value.ToString();
            stringBuilder.Append(paramterValueString);
            stringBuilder.Append(", ");
        }

        if (parameters.Count() > 0)
        {
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
        }

        return stringBuilder.ToString();
    }
}
