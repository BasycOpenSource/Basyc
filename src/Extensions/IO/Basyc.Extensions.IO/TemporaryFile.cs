namespace Basyc.Extensions.IO;

public readonly record struct TemporaryFile(string FullPath) : IDisposable
{
	public void Dispose()
	{
		System.IO.File.Delete(FullPath);
	}

	public static string GetNewTempFilename(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp")
	{
		string tempDirectory = Path.GetTempPath();
		string fileName = $"{nameFriendlyPart}_{Guid.NewGuid():D}.{fileExtension}";
		string fileFullPath = Path.Combine(tempDirectory, fileName);
		if (System.IO.File.Exists(fileFullPath))
		{
			throw new InvalidOperationException("Failed to create temp file. File already exits!");
		}

		return fileFullPath;
	}

	public static TemporaryFile CreateTempFile(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp")
	{
		string fileFullPath = GetNewTempFilename(nameFriendlyPart, fileExtension);
		System.IO.File.Create(fileFullPath).Dispose();
		return new TemporaryFile(fileFullPath);
	}
}
