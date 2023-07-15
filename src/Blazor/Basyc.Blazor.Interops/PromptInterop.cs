using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace Basyc.Blazor.Interops;
[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Source generator.")]
public partial class PromptInterop
{
    [JSImport("getMessage", "promptInterop")]
    public static partial string GetWelcomeMessage();
}
