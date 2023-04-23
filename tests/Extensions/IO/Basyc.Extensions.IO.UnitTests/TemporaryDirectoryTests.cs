namespace Basyc.Extensions.IO.UnitTests;
public class TemporaryDirectoryTests : IDisposable
{
    private readonly List<string> dirsToCleanAfterTest = new();

    [Fact]
    public void GetNewName_Should_GiveUniqueName()
    {
        string[] names = Enumerable.Range(0, 10).Select(x => TemporaryDirectory.GetNewPath()).ToArray();
        names.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetNewName_Should_GiveUniqueName_With_Selected_FriendlyName()
    {
        string friendlyName = "friendlyname";
        string[] names = Enumerable.Range(0, 10).Select(x => TemporaryDirectory.GetNewPath(directoryNameStart: friendlyName)).ToArray();
        names.Should().OnlyHaveUniqueItems();
        names.Should().AllSatisfy(x => x.Should().Contain(friendlyName));
    }

    [Fact]
    public void CreateNew_Should_CreateWithUniqueName()
    {
        var temporaryDirs = Enumerable.Range(0, 10).Select(x => TemporaryDirectory.CreateNew()).ToArray();
        Array.ForEach(temporaryDirs, x => dirsToCleanAfterTest.Add(x.FullPath));
        temporaryDirs.Should().OnlyHaveUniqueItems();
        bool[] exists = temporaryDirs.Select(x => Directory.Exists(x.FullPath)).ToArray();
        exists.Should().OnlyContain(x => x == true);
    }

    [Fact]
    public void Dispose_Should_DeleteDirectory()
    {
        var tempDir = TemporaryDirectory.CreateNew();
        dirsToCleanAfterTest.Add(tempDir.FullPath);
        tempDir.Dispose();
        Directory.Exists(tempDir.FullPath).Should().BeFalse();
    }

    [Fact]
    public void GetInfo_Should_NotThrow()
    {
        var tempDir = TemporaryDirectory.CreateNew();
        dirsToCleanAfterTest.Add(tempDir.FullPath);
        tempDir.GetInfo();
    }

    [Fact]
    public void CreateFromExisting_Should_Throw_When_DirectoryDoesNotExists()
    {
        var action = () => TemporaryDirectory.CreateFromExisting(Path.GetTempPath() + "notExistingDir");
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void CreateFromExisting_Should_NotThrow_When_DirectoryExists()
    {
        using var newDir = TemporaryDirectory.CreateNew();
        dirsToCleanAfterTest.Add(newDir.FullPath);
        var action = () => TemporaryDirectory.CreateFromExisting(newDir.FullPath);
        action.Should().NotThrow<Exception>();
    }

    public void Dispose() => dirsToCleanAfterTest.ForEach(x =>
                                  {
                                      if (Directory.Exists(x))
                                      {
                                          Directory.Delete(x, true);
                                      }
                                  });
}
