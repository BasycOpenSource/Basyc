using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;
//Tool used: http://xmltocsharp.azurewebsites.net/

[XmlRoot(ElementName = "CoverageSession")]
public class CoverageSession
{
    [XmlElement(ElementName = "Summary")]
    public Summary? Summary { get; set; }

    [XmlElement(ElementName = "Modules")]
    public Modules? Modules { get; set; }
}
