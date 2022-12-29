using Basyc.Extensions.Nuke.Tasks.Dotnet.Format;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{
	public static void BasycNugetSign(IEnumerable<string> paths, string certPath, string? certPassword)
	{
		DotnetWrapper.NugetSign(paths, certPath, certPassword);
	}

	public static void BasycNugetSign(string path, string certPath, string? certPassword)
	{
		DotnetWrapper.NugetSign(new[] { path }, certPath, certPassword);
	}
}
