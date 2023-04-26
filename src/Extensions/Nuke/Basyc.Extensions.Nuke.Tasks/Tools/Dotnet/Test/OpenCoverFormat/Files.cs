using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only

[XmlRoot(ElementName = "Files")]
public class Files
{
    [XmlElement(ElementName = "File")]
    public List<File>? File { get; set; }
}
