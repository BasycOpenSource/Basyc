using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Basyc.Blazor.Interops;
[SupportedOSPlatform("browser")]
[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Source generator.")]
public partial class PromptInterop
{
    //[JSImport("getMessage", "promptInterop")]
    //[JSImport("getMessage", "_content/Basyc.Blazor.Interops/promptInterop.js")]
    //[JSImport("getMessage", "../_content/Basyc.Blazor.Interops/promptInterop.js")]
    //[JSImport("getMessage", "Basyc.Blazor.Interops.promptInterop")]
    [JSImport("getMessage", "_content/Basyc.Blazor.Interops/promptInterop")]
    public static partial string GetWelcomeMessage();
}
