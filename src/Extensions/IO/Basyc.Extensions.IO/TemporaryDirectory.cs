namespace Basyc.Extensions.IO;

public readonly record struct TemporaryDirectory(string FullPath) : IDisposable
{
	public DirectoryInfo GetInfo()
	{
		return new DirectoryInfo(FullPath);
	}

	public static string GetNewName(string directoryNameStart = "Basyc_temp_dir", bool includeUniqueNumber = true)
	{
		string tempDirectory = Path.GetTempPath();
		string? directoryName = includeUniqueNumber ? $"{directoryNameStart}_{Guid.NewGuid():D}" : directoryNameStart;
		string directoryFullPath = Path.Combine(tempDirectory, directoryName) + Path.DirectorySeparatorChar;
		if (Directory.Exists(directoryFullPath))
		{
			throw new InvalidOperationException("Failed to create temp directory. Directory already exits!");
		}

		return directoryFullPath;
	}

	public static TemporaryDirectory CreateNew(string directoryNameStart = "Basyc_temp_dir", bool includeUniqueNumber = true)
	{
		string directoryFullPath = GetNewName(directoryNameStart, includeUniqueNumber);
		Directory.CreateDirectory(directoryFullPath);
		return new TemporaryDirectory(directoryFullPath);
	}

	public void Dispose()
	{
		Directory.Delete(FullPath, true);
	}
}
