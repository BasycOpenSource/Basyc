using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Methods")]
public class Methods
{
    [XmlElement(ElementName = "Method")]
    public List<Method>? Method { get; set; }
}
