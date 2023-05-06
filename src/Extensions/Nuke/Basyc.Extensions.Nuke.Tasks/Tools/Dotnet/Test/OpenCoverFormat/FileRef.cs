using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "FileRef")]
public class FileRef
{
    [XmlAttribute(AttributeName = "uid")]
    public string? Uid { get; set; }
}
