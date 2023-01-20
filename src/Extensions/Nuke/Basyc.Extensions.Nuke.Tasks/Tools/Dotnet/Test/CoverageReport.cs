﻿using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
public record CoverageReport(
	TemporaryDirectory Directory,
	ProjectCoverageReport[] Projects,
	Dictionary<string, TemporaryFile> ProjectToCoverageFileMap)
	: IDisposable
{
	public void Dispose()
	{
		Directory.Dispose();
	}
}

