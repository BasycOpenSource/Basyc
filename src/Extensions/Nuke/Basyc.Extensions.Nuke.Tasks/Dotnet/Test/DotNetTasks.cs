using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks;
public static partial class DotNetTasks
{
	public static void UnitTestAndCoverage(IEnumerable<string> testProjectsPaths)
	{
		//DotnetWrapper.Test(pathToDll, "XPlat Code Coverage");
		string settingsPath = CreateRunSettings(testProjectsPaths).FullName;
		DotNetTest(_ => _
			.EnableNoRestore()
			.EnableNoBuild()
			.EnableCollectCoverage() //dotnet add package coverlet.msbuild to only test projects
			.AddProperty("CoverletOutputFormat", "opencover")
			.SetSettingsFile(settingsPath)
			.CombineWith(testProjectsPaths,
		(settings, unitTestProject) => settings
				.SetProjectFile(unitTestProject)),
			degreeOfParallelism: 5);

		File.Delete(settingsPath);
	}

	private static FileInfo CreateRunSettings(IEnumerable<string> testProjectPaths)
	{
		string includeParam = string.Join(",", testProjectPaths
			.Select(x => $"[{Path.GetFileNameWithoutExtension(x)}*]*"));

		//Example and more options here:
		//https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
		string fileContent = $"""
			<?xml version="1.0" encoding="utf-8" ?>
			<RunSettings>
			  <DataCollectionRunSettings>
			    <DataCollectors>
			      <DataCollector friendlyName="XPlat code coverage">
			        <Configuration>
			          <Format>opencover</Format>          
					  <Include>{includeParam}</Include> <!-- [Assembly-Filter]Type-Filter -->
			          <Exclude>[*test*]*</Exclude> <!-- [Assembly-Filter]Type-Filter -->
			          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
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

		string filePath = $"{Path.GetTempPath()}/coverlet_{Guid.NewGuid():D}.runsettings";
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}

		File.WriteAllText(filePath, fileContent);

		return new FileInfo(filePath);
	}
}
