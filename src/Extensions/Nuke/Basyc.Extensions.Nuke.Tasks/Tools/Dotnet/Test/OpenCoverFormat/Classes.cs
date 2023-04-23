using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Classes")]
public class Classes
{
    [XmlElement(ElementName = "Class")]
    public List<Class>? Class { get; set; }
}
