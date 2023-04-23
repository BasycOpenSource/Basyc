using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Class")]
public class Class
{
    [XmlElement(ElementName = "Summary")]
    public Summary? Summary { get; set; }

    [XmlElement(ElementName = "FullName")]
    public string? FullName { get; set; }

    [XmlElement(ElementName = "Methods")]
    public Methods? Methods { get; set; }
}
