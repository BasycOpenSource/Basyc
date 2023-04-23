using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Modules")]
public class Modules
{
    [XmlElement(ElementName = "Module")]
    public List<Module>? Module { get; set; }
}
