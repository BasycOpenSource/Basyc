using Basyc.Extensions.IO;
using Nuke.Common.Utilities.Collections;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;

public readonly struct TestRunSettingsMultiple : IDisposable
{
	private readonly Dictionary<string, TemporaryFile> temporaryFiles = new();

	public TestRunSettingsMultiple(IEnumerable<string> projectToTestNames)
	{
		foreach (var projectToTestName in projectToTestNames)
		{
			temporaryFiles.Add(projectToTestName, CreateRunSettings(projectToTestName));
		}
	}

	public void Dispose()
	{
		temporaryFiles.Values.ForEach(x => x.Dispose());
	}

	public TemporaryFile GetForProject(string projectToTestFullPath)
	{
		return temporaryFiles[projectToTestFullPath];
	}

	//Example and more options here:
	//https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
	private static TemporaryFile CreateRunSettings(params string[] projectToTestNames)
	{
		var includeParam = string.Join(",", projectToTestNames.Select(x => $"[{x}]*"));
		var fileContent = $"""
		<?xml version="1.0" encoding="utf-8" ?>
			<RunSettings>
			  <DataCollectionRunSettings>
			    <DataCollectors>
			      <DataCollector friendlyName="XPlat code coverage">
			        <Configuration>
			          <Format>opencover</Format>
					  <Include>{includeParam}</Include> <!-- [Assembly-Filter]Type-Filter -->
			          <Exclude>[*test*]*</Exclude> <!-- [Assembly-Filter]Type-Filter -->
			          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute,ExcludeFromCodeCoverageAttribute</ExcludeByAttribute>
			          <SingleHit>false</SingleHit>
			          <UseSourceLink>true</UseSourceLink>
			          <IncludeTestAssembly>true</IncludeTestAssembly>
			          <SkipAutoProps>true</SkipAutoProps>
			          <DeterministicReport>false</DeterministicReport>
			          <ExcludeAssembliesWithoutSources>MissingAll,MissingAny,None</ExcludeAssembliesWithoutSources>
			        </Configuration>
			      </DataCollector>
			    </DataCollectors>
			  </DataCollectionRunSettings>
			</RunSettings>
		""";

		var settingFile = TemporaryFile.CreateNewWith("coverlet", "runsettings", fileContent);
		return settingFile;
	}
}
