using System.Diagnostics.CodeAnalysis;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
namespace _build;

public static partial class DotNetTasks
{
    public static bool DotnetFormatVerifyNoChanges(string project, [NotNullWhen(false)] out string? errorMessage, bool throwOnNotFormatted = true)
    {
        try
        {
            DotNet($"format {project} --verify-no-changes");
            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            if (throwOnNotFormatted)
            {
                throw;
            }

            errorMessage = ex.Message;
            return false;
        }
    }
}