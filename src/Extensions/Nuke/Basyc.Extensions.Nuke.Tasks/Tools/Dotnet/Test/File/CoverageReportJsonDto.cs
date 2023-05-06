using Basyc.Extensions.IO;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.File;

#pragma warning disable CA1819 // Properties should not return arrays

public class CoverageReportJsonDto
{
    public CoverageReportJsonDto(ProjectCoverageReport[] projects)
    {
        Projects = projects;
    }

    public ProjectCoverageReport[] Projects { get; init; }

    public static CoverageReportJsonDto ToDto(CoverageReport coverageReport)
    {
        var dto = new CoverageReportJsonDto(coverageReport.Projects);
        return dto;
    }

    public static CoverageReport ToReport(CoverageReportJsonDto dto) => new(TemporaryDirectory.CreateNew(), dto.Projects);
}
