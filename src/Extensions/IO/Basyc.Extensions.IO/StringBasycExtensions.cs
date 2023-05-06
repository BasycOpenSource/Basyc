namespace Basyc.Extensions.IO;

public static class StringBasycExtensions
{
    /// <summary>
    ///     Change all \ to /.
    /// </summary>
    public static string NormalizePath(this string path) => path.Replace('\\', '/');

    public static string NormalizeForCurrentOs(this string path) => path.Replace('\\', '/').Replace('/', Path.DirectorySeparatorChar);
}
