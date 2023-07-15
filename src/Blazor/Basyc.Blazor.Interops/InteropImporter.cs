using System.Runtime.InteropServices.JavaScript;

namespace Basyc.Blazor.Interops;
public static class InteropImporter
{
#pragma warning disable CA1416 // Validate platform compatibility
    public static async Task ImportJavaScriptModules()
    {
        var contentFolder = "../_content/Basyc.Blazor.Interops";
        var interopNames = typeof(InteropImporter).Assembly.GetTypes()
            .Where(x => x.Name.EndsWith("Interop"))
            .Select(x => string.Concat(char.ToLower(x.Name.First()), x.Name.Substring(1)));
        foreach (var interopName in interopNames)
        {
            await JSHost.ImportAsync(interopName, $"{contentFolder}/{interopName}.js");
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility
}
