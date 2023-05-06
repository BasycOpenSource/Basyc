using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Summary")]
public class Summary
{
    [XmlAttribute(AttributeName = "numSequencePoints")]
    public string? NumSequencePoints { get; set; }

    [XmlAttribute(AttributeName = "visitedSequencePoints")]
    public string? VisitedSequencePoints { get; set; }

    [XmlAttribute(AttributeName = "numBranchPoints")]
    public string? NumBranchPoints { get; set; }

    [XmlAttribute(AttributeName = "visitedBranchPoints")]
    public string? VisitedBranchPoints { get; set; }

    [XmlAttribute(AttributeName = "sequenceCoverage")]
    public string? SequenceCoverage { get; set; }

    [XmlAttribute(AttributeName = "branchCoverage")]
    public string? BranchCoverage { get; set; }

    [XmlAttribute(AttributeName = "maxCyclomaticComplexity")]
    public string? MaxCyclomaticComplexity { get; set; }

    [XmlAttribute(AttributeName = "minCyclomaticComplexity")]
    public string? MinCyclomaticComplexity { get; set; }

    [XmlAttribute(AttributeName = "visitedClasses")]
    public string? VisitedClasses { get; set; }

    [XmlAttribute(AttributeName = "numClasses")]
    public string? NumClasses { get; set; }

    [XmlAttribute(AttributeName = "visitedMethods")]
    public string? VisitedMethods { get; set; }

    [XmlAttribute(AttributeName = "numMethods")]
    public string? NumMethods { get; set; }
}
