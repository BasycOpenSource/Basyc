using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

#pragma warning disable CA1724 // class name same as namespace part

[XmlRoot(ElementName = "File")]
public class File
{
    [XmlAttribute(AttributeName = "uid")]
    public string? Uid { get; set; }

    [XmlAttribute(AttributeName = "fullPath")]
    public string? FullPath { get; set; }
}
