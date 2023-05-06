using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "Module")]
public class Module
{
    [XmlElement(ElementName = "ModulePath")]
    public string? ModulePath { get; set; }

    [XmlElement(ElementName = "ModuleTime")]
    public string? ModuleTime { get; set; }

    [XmlElement(ElementName = "ModuleName")]
    public string? ModuleName { get; set; }

    [XmlElement(ElementName = "Files")]
    public Files? Files { get; set; }

    [XmlElement(ElementName = "Classes")]
    public Classes? Classes { get; set; }

    [XmlAttribute(AttributeName = "hash")]
    public string? Hash { get; set; }
}
