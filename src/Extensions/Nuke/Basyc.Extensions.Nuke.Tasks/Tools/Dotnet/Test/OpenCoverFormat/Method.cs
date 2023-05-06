using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Method")]
public class Method
{
    [XmlElement(ElementName = "Summary")]
    public Summary? Summary { get; set; }

    [XmlElement(ElementName = "MetadataToken")]
    public string? MetadataToken { get; set; }

    [XmlElement(ElementName = "Name")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "FileRef")]
    public FileRef? FileRef { get; set; }

    [XmlElement(ElementName = "SequencePoints")]
    public SequencePoints? SequencePoints { get; set; }

    [XmlElement(ElementName = "BranchPoints")]
    public BranchPoints? BranchPoints { get; set; }

    [XmlElement(ElementName = "MethodPoint")]
    public MethodPoint? MethodPoint { get; set; }

    [XmlAttribute(AttributeName = "cyclomaticComplexity")]
    public string? CyclomaticComplexity { get; set; }

    [XmlAttribute(AttributeName = "nPathComplexity")]
    public string? NPathComplexity { get; set; }

    [XmlAttribute(AttributeName = "sequenceCoverage")]
    public string? SequenceCoverage { get; set; }

    [XmlAttribute(AttributeName = "branchCoverage")]
    public string? BranchCoverage { get; set; }

    [XmlAttribute(AttributeName = "isConstructor")]
    public string? IsConstructor { get; set; }

    [XmlAttribute(AttributeName = "isGetter")]
    public string? IsGetter { get; set; }

    [XmlAttribute(AttributeName = "isSetter")]
    public string? IsSetter { get; set; }

    [XmlAttribute(AttributeName = "isStatic")]
    public string? IsStatic { get; set; }
}
