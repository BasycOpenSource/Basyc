﻿namespace Basyc.Extensions.IO;

#pragma warning disable CA2225 // Operator overloads have named alternates

public readonly record struct TemporaryFile(string FullPath) : IDisposable
{
    public static explicit operator TemporaryFile(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        return new TemporaryFile(path);
    }

    public static implicit operator string(TemporaryFile path) => path.ToString();

    public void Dispose() => File.Delete(FullPath);

    public static string GetNew(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp", string? folderPath = null)
    {
        folderPath ??= Path.GetTempPath();
        string fileName = $"{nameFriendlyPart}_{Guid.NewGuid():D}.{fileExtension}";
        string fileFullPath = Path.Combine(folderPath, fileName);
        if (File.Exists(fileFullPath))
        {
            throw new InvalidOperationException("Failed to create temp file. File already exits!");
        }

        return fileFullPath;
    }

    public static TemporaryFile CreateNew(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp")
    {
        string fileFullPath = GetNew(nameFriendlyPart, fileExtension);
        File.Create(fileFullPath).Dispose();
        return new TemporaryFile(fileFullPath);
    }

    public static TemporaryFile CreateNewWith(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp", string? content = null)
    {
        string fileFullPath = GetNew(nameFriendlyPart, fileExtension);
        File.Create(fileFullPath).Dispose();
        File.WriteAllText(fileFullPath, content);
        return new TemporaryFile(fileFullPath);
    }

    public static TemporaryFile CreateNewWith(string nameFriendlyPart = "Basyc_temp_file", string? fileExtension = "tmp", byte[]? content = null)
    {
        string fileFullPath = GetNew(nameFriendlyPart, fileExtension);
        File.Create(fileFullPath).Dispose();
        if (content is not null)
        {
            File.WriteAllBytes(fileFullPath, content);
        }

        return new TemporaryFile(fileFullPath);
    }

    public static TemporaryFile CreateFromExisting(string fullPath) => new(fullPath);
}
