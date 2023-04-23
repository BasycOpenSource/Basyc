using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

[XmlRoot(ElementName = "BranchPoints")]
public class BranchPoints
{
    [XmlElement(ElementName = "BranchPoint")]
    public List<BranchPoint>? BranchPoint { get; set; }
}
