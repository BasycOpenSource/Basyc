using Nuke.Common.Tooling;
using System.Diagnostics.Contracts;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Format;

[Serializable]
public class DotNetFormatSettings : ToolSettings
{
	public override Action<OutputType, string> ProcessCustomLogger => DotNetLogger;
	public virtual string? Project { get; internal set; }
	public virtual List<string> Include { get; internal set; } = new List<string>();

	[Pure]
	public DotNetFormatSettings SetProject(string project)
	{
		var settings = this.NewInstance();
		settings.Project = project;
		return settings;
	}

	[Pure]
	public DotNetFormatSettings AddInclude(IEnumerable<string> filesToInclude)
	{
		var settings = this.NewInstance();
		settings.Include.AddRange(filesToInclude);
		return settings;
	}

	[Pure]
	public DotNetFormatSettings AddInclude(params string[] filesToInclude)
	{
		return AddInclude(filesToInclude.AsEnumerable());
	}
}
