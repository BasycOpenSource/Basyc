﻿using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;

#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only

[XmlRoot(ElementName = "BranchPoints")]
public class BranchPoints
{
    [XmlElement(ElementName = "BranchPoint")]
    public List<BranchPoint>? BranchPoint { get; set; }
}
