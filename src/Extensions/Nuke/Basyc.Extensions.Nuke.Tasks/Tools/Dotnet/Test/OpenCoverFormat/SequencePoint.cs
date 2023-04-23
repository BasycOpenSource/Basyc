using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "SequencePoint")]
public class SequencePoint
{
    [XmlAttribute(AttributeName = "vc")]
    public string? Vc { get; set; }

    [XmlAttribute(AttributeName = "uspid")]
    public string? Uspid { get; set; }

    [XmlAttribute(AttributeName = "ordinal")]
    public string? Ordinal { get; set; }

    [XmlAttribute(AttributeName = "sl")]
    public string? Sl { get; set; }

    [XmlAttribute(AttributeName = "sc")]
    public string? Sc { get; set; }

    [XmlAttribute(AttributeName = "el")]
    public string? El { get; set; }

    [XmlAttribute(AttributeName = "ec")]
    public string? Ec { get; set; }

    [XmlAttribute(AttributeName = "bec")]
    public string? Bec { get; set; }

    [XmlAttribute(AttributeName = "bev")]
    public string? Bev { get; set; }

    [XmlAttribute(AttributeName = "fileid")]
    public string? Fileid { get; set; }
}
