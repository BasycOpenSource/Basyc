using Basyc.Extensions.Nuke.Targets.Nuget;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuilds : IBasycBuildCommonAffected, IBasycBuildCommonAll, IBasycBuildNugetAll
{
}
