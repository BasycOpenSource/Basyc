namespace Basyc.Extensions.IO;

public readonly record struct TemporaryDirectory(string FullPath) : IDisposable
{
	public DirectoryInfo GetInfo()
	{
		return new DirectoryInfo(FullPath);
	}

	public static string GetNewPath(string directoryNameStart = "Basyc_temp_dir", bool includeUniqueNumber = true)
	{
		string tempDirectory = Path.Combine(Path.GetTempPath(), "Basyc");
		string? directoryName = includeUniqueNumber ? $"{directoryNameStart}_{Guid.NewGuid():D}" : directoryNameStart;
		string directoryFullPath = Path.Combine(tempDirectory, directoryName) + Path.DirectorySeparatorChar;
		if (includeUniqueNumber && Directory.Exists(directoryFullPath))
		{
			throw new InvalidOperationException("Failed to create unique temp directory. Directory already exists!");
		}

		return directoryFullPath;
	}

	public static TemporaryDirectory CreateNew(string directoryNameStart = "Basyc_temp_dir", bool includeUniqueNumber = true)
	{
		string directoryFullPath = GetNewPath(directoryNameStart, includeUniqueNumber);
		Directory.CreateDirectory(directoryFullPath);
		return new TemporaryDirectory(directoryFullPath);
	}

	public static TemporaryDirectory CreateFromExisting(string directoryFullPath)
	{
		if (Directory.Exists(directoryFullPath) is false)
		{
			throw new InvalidOperationException("Directory does not exist");
		}

		return new TemporaryDirectory(directoryFullPath);
	}

	public TemporaryFile CreateFile(string fileName)
	{
		var temporaryFile = TemporaryFile.CreateNew(Path.Combine(FullPath, fileName));
		return temporaryFile;
	}

	public void Dispose()
	{
		Directory.Delete(FullPath, true);
	}
}
