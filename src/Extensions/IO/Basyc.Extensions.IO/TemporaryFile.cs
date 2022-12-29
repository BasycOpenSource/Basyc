namespace Basyc.Extensions.IO;

public readonly record struct TemporaryFile(string FullPath) : IDisposable
{
	public void Dispose()
	{
		File.Delete(FullPath);
	}

	public static string GetNewName(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp")
	{
		string tempDirectory = Path.GetTempPath();
		string fileName = $"{nameFriendlyPart}_{Guid.NewGuid():D}.{fileExtension}";
		string fileFullPath = Path.Combine(tempDirectory, fileName);
		if (File.Exists(fileFullPath))
		{
			throw new InvalidOperationException("Failed to create temp file. File already exits!");
		}

		return fileFullPath;
	}

	public static TemporaryFile CreateNew(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp")
	{
		string fileFullPath = GetNewName(nameFriendlyPart, fileExtension);
		File.Create(fileFullPath).Dispose();
		return new TemporaryFile(fileFullPath);
	}

	public static TemporaryFile CreateNewWith(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp", string? content = null)
	{
		string fileFullPath = GetNewName(nameFriendlyPart, fileExtension);
		File.Create(fileFullPath).Dispose();
		File.WriteAllText(fileFullPath, content);
		return new TemporaryFile(fileFullPath);
	}

	public static TemporaryFile CreateNewWith(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp", byte[]? content = null)
	{
		string fileFullPath = GetNewName(nameFriendlyPart, fileExtension);
		File.Create(fileFullPath).Dispose();
		if (content is not null)
		{
			File.WriteAllBytes(fileFullPath, content);
		}

		return new TemporaryFile(fileFullPath);
	}
}
