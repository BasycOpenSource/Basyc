using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Files")]
public class Files
{
    [XmlElement(ElementName = "File")]
    public List<File>? File { get; set; }
}
