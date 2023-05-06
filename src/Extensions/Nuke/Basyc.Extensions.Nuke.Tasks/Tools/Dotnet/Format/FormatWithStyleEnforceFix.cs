using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Format;

/// <summary>
/// Cant run dotnet format when EnforceCodeStyleInBuild is true
/// https://github.com/dotnet/format/issues/1805
/// https://github.com/dotnet/format/issues/1800.
/// </summary>
public class FormatWithStyleEnforceFix : IDisposable
{
    private readonly TemporaryFile tempTargetsFile;

    private FormatWithStyleEnforceFix(TemporaryFile temporaryFile)
    {
        tempTargetsFile = temporaryFile;
    }

    public static FormatWithStyleEnforceFix Fix(string projectOrSolutionPath)
    {
        var projectOrSolutionDirectory = Directory.GetParent(projectOrSolutionPath).Value();
        var tempTargetsFilePath = $"{projectOrSolutionDirectory.FullName}{Path.DirectorySeparatorChar}Directory.Build.targets";
        if (File.Exists(tempTargetsFilePath))
        {
            throw new InvalidOperationException($"{tempTargetsFilePath} is not expected to exists");
        }

        var tempTargetsFileContent =
            """
            <Project>
              <PropertyGroup>
                <EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>
              </PropertyGroup>
            </Project>
            """;
        File.WriteAllText(tempTargetsFilePath, tempTargetsFileContent);
        var tempTargetsFile = new TemporaryFile(tempTargetsFilePath);
        return new FormatWithStyleEnforceFix(tempTargetsFile);
    }

    public void Dispose() => tempTargetsFile.Dispose();
}
