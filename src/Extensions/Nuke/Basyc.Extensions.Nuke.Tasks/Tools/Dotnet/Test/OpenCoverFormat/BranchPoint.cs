using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "BranchPoint")]
public class BranchPoint
{
    [XmlAttribute(AttributeName = "vc")]
    public string? Vc { get; set; }

    [XmlAttribute(AttributeName = "uspid")]
    public string? Uspid { get; set; }

    [XmlAttribute(AttributeName = "ordinal")]
    public string? Ordinal { get; set; }

    [XmlAttribute(AttributeName = "path")]
    public string? Path { get; set; }

    [XmlAttribute(AttributeName = "offset")]
    public string? Offset { get; set; }

    [XmlAttribute(AttributeName = "offsetend")]
    public string? Offsetend { get; set; }

    [XmlAttribute(AttributeName = "sl")]
    public string? Sl { get; set; }

    [XmlAttribute(AttributeName = "fileid")]
    public string? Fileid { get; set; }
}
