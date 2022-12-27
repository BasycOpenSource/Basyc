namespace Basyc.Extensions.IO;
public static class StringBasycExtensions
{
	/// <summary>
	/// Change all \ to /
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static string NormalizePath(this string path)
	{
		return path.Replace("\\", "/");
	}
}
