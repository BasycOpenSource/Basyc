﻿namespace Basyc.Extensions.IO.UnitTests;
public class TemporaryDirectoryTests : IDisposable
{
	private readonly List<string> dirsToCleanAfterTest = new();

	[Fact]
	public void GetNewName_Should_GiveUniqueName()
	{
		string[] names = Enumerable.Range(0, 10).Select(x => TemporaryDirectory.GetNewName()).ToArray();
		names.Should().OnlyHaveUniqueItems();
	}

	[Fact]
	public void GetNewName_Should_GiveUniqueName_With_Selected_FriendlyName()
	{
		string friendlyName = "friendlyname";
		string[] names = Enumerable.Range(0, 10).Select(x => TemporaryDirectory.GetNewName(directoryNameStart: friendlyName)).ToArray();
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

	public void Dispose()
	{
		dirsToCleanAfterTest.ForEach(x =>
		{
			if (Directory.Exists(x))
			{
				Directory.Delete(x, true);
			}
		});
	}
}
