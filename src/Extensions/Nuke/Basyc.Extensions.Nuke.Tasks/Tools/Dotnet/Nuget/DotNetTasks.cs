using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet;

namespace Basyc.Extensions.Nuke.Tasks;

public static class DotNetTasks
{
    public static void BasycNugetSignWithFile(IEnumerable<string> paths, string certPath, string? certPassword) => DotnetCliWrapper.NugetSignWithFile(paths, certPath, certPassword);

    public static void BasycNugetSignWithFile(string path, string certPath, string? certPassword) => DotnetCliWrapper.NugetSignWithFile(new[] { path }, certPath, certPassword);

    public static void BasycNugetSignWithBase64(string path, string base64Cert, string? certPassword) => DotnetCliWrapper.NugetSignWithFile(new[] { path }, base64Cert, certPassword);
}
