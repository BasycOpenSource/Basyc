namespace Basyc.Extensions.IO.UnitTests;
public class TemporaryFileTests : IDisposable
{
    private readonly List<string> filesToCleanAfterTest = new();

    [Fact]
    public void GetNew_Should_GiveUniqueName()
    {
        string[] names = Enumerable.Range(0, 10).Select(x => TemporaryFile.GetNew()).ToArray();
        names.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetNew_Should_GiveUniqueName_With_Selected_FriendlyName()
    {
        string friendlyName = "friendlyname";
        string[] names = Enumerable.Range(0, 10).Select(x => TemporaryFile.GetNew(nameFriendlyPart: friendlyName)).ToArray();
        names.Should().OnlyHaveUniqueItems();
        names.Should().AllSatisfy(x => x.Should().Contain(friendlyName));
    }

    [Fact]
    public void GetNew_Should_GiveUniqueName_With_Selected_Extension()
    {
        string extension = "tmp2";
        string[] names = Enumerable.Range(0, 10).Select(x => TemporaryFile.GetNew(fileExtension: extension)).ToArray();
        names.Should().OnlyHaveUniqueItems();
        names.Should().AllSatisfy(x => x.Should().EndWith($".{extension}"));
    }

    [Fact]
    public void CreateNew_Should_CreateFileWithUniqueName()
    {
        var temporaryFiles = Enumerable.Range(0, 10).Select(x => TemporaryFile.CreateNew()).ToArray();
        Array.ForEach(temporaryFiles, x => filesToCleanAfterTest.Add(x.FullPath));

        temporaryFiles.Should().OnlyHaveUniqueItems();
        temporaryFiles.Should().AllSatisfy(x => File.Exists(x.FullPath));
    }

    [Fact]
    public void Dispose_Should_DeleteFile()
    {
        var tempFile = TemporaryFile.CreateNew();
        filesToCleanAfterTest.Add(tempFile.FullPath);
        tempFile.Dispose();
        File.Exists(tempFile.FullPath).Should().BeFalse();
    }

    public void Dispose() => filesToCleanAfterTest.ForEach(File.Delete);
}
