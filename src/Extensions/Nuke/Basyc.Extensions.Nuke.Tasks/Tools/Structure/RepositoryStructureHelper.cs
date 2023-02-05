using Nuke.Common.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Structure;

//Based on https://gist.github.com/davidfowl/ed7564297c61fe9ab814
// artifacts/
// build/
// docs/
// lib/
// packages/
// samples/
// src/
// tests/
// .editorconfig
// .gitignore
// .gitattributes
// build.cmd
// build.sh
// LICENSE
// NuGet.Config
// README.md
// {solution}.sln
public static class RepositoryStructureHelper
{
	public static AbsolutePath GetRootFolder(string rootDirectory)
	{
		return (AbsolutePath)rootDirectory;
	}


	public static AbsolutePath GetDocsFolder(string rootDirectory)
	{
		return GetRootFolder(rootDirectory) / "docs";
	}

	public static AbsolutePath GetSourceFolder(string rootDirectory)
	{
		return GetRootFolder(rootDirectory) / "src";
	}

	public static AbsolutePath GetTestsFolder(string rootDirectory)
	{
		return GetRootFolder(rootDirectory) / "tests";
	}

	public static AbsolutePath GetTestsHistoryFolder(string rootDirectory)
	{
		return GetTestsFolder(rootDirectory) / "history";
	}
}
