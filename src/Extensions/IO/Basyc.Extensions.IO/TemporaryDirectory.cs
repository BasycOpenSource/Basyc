namespace Basyc.Extensions.IO;

public readonly record struct TemporaryDirectory(string FullPath) : IDisposable
{
	public DirectoryInfo GetInfo()
	{
		return new DirectoryInfo(FullPath);
	}
	public void Dispose()
	{
		System.IO.Directory.Delete(FullPath, true);
	}

	public static string GetNewTempDirectory(string directoryNameStart = "Basyc_temp_dir", bool includeUniqueNumber = true)
	{
		string tempDirectory = Path.GetTempPath();
		string? directoryName = includeUniqueNumber ? $"{directoryNameStart}_{Guid.NewGuid():D}" : directoryNameStart;
		string directoryFullPath = Path.Combine(tempDirectory, directoryName) + Path.DirectorySeparatorChar;
		if (System.IO.Directory.Exists(directoryFullPath))
		{
			throw new InvalidOperationException("Failed to create temp directory. Directory already exits!");
		}

		return directoryFullPath;
	}

	public static TemporaryDirectory CreateTempDirectory(string directoryNameStart = "Basyc_temp_dir", bool includeUniqueNumber = true)
	{
		string directoryFullPath = GetNewTempDirectory(directoryNameStart, includeUniqueNumber);
		System.IO.Directory.CreateDirectory(directoryFullPath);
		return new TemporaryDirectory(directoryFullPath);
	}
}
