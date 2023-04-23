using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "SequencePoints")]
public class SequencePoints
{
    [XmlElement(ElementName = "SequencePoint")]
    public List<SequencePoint>? SequencePoint { get; set; }
}
