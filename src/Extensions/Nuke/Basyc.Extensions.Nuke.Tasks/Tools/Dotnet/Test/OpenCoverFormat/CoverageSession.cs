using System.Xml.Serialization;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test.OpenCoverFormat;
//Tool used: http://xmltocsharp.azurewebsites.net/

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

[XmlRoot(ElementName = "Summary")]
public class Summary
{
	[XmlAttribute(AttributeName = "numSequencePoints")]
	public string? NumSequencePoints { get; set; }
	[XmlAttribute(AttributeName = "visitedSequencePoints")]
	public string? VisitedSequencePoints { get; set; }
	[XmlAttribute(AttributeName = "numBranchPoints")]
	public string? NumBranchPoints { get; set; }
	[XmlAttribute(AttributeName = "visitedBranchPoints")]
	public string? VisitedBranchPoints { get; set; }
	[XmlAttribute(AttributeName = "sequenceCoverage")]
	public string SequenceCoverage { get; set; }
	[XmlAttribute(AttributeName = "branchCoverage")]
	public string BranchCoverage { get; set; }
	[XmlAttribute(AttributeName = "maxCyclomaticComplexity")]
	public string? MaxCyclomaticComplexity { get; set; }
	[XmlAttribute(AttributeName = "minCyclomaticComplexity")]
	public string? MinCyclomaticComplexity { get; set; }
	[XmlAttribute(AttributeName = "visitedClasses")]
	public string? VisitedClasses { get; set; }
	[XmlAttribute(AttributeName = "numClasses")]
	public string? NumClasses { get; set; }
	[XmlAttribute(AttributeName = "visitedMethods")]
	public string? VisitedMethods { get; set; }
	[XmlAttribute(AttributeName = "numMethods")]
	public string? NumMethods { get; set; }
}

[XmlRoot(ElementName = "File")]
public class File
{
	[XmlAttribute(AttributeName = "uid")]
	public string? Uid { get; set; }
	[XmlAttribute(AttributeName = "fullPath")]
	public string? FullPath { get; set; }
}

[XmlRoot(ElementName = "Files")]
public class Files
{
	[XmlElement(ElementName = "File")]
	public List<File>? File { get; set; }
}

[XmlRoot(ElementName = "FileRef")]
public class FileRef
{
	[XmlAttribute(AttributeName = "uid")]
	public string? Uid { get; set; }
}

[XmlRoot(ElementName = "SequencePoint")]
public class SequencePoint
{
	[XmlAttribute(AttributeName = "vc")]
	public string? Vc { get; set; }
	[XmlAttribute(AttributeName = "uspid")]
	public string? Uspid { get; set; }
	[XmlAttribute(AttributeName = "ordinal")]
	public string? Ordinal { get; set; }
	[XmlAttribute(AttributeName = "sl")]
	public string? Sl { get; set; }
	[XmlAttribute(AttributeName = "sc")]
	public string? Sc { get; set; }
	[XmlAttribute(AttributeName = "el")]
	public string? El { get; set; }
	[XmlAttribute(AttributeName = "ec")]
	public string? Ec { get; set; }
	[XmlAttribute(AttributeName = "bec")]
	public string? Bec { get; set; }
	[XmlAttribute(AttributeName = "bev")]
	public string? Bev { get; set; }
	[XmlAttribute(AttributeName = "fileid")]
	public string? Fileid { get; set; }
}

[XmlRoot(ElementName = "SequencePoints")]
public class SequencePoints
{
	[XmlElement(ElementName = "SequencePoint")]
	public List<SequencePoint>? SequencePoint { get; set; }
}

[XmlRoot(ElementName = "BranchPoint")]
public class BranchPoint
{
	[XmlAttribute(AttributeName = "vc")]
	public string? Vc { get; set; }
	[XmlAttribute(AttributeName = "uspid")]
	public string? Uspid { get; set; }
	[XmlAttribute(AttributeName = "ordinal")]
	public string? Ordinal { get; set; }
	[XmlAttribute(AttributeName = "path")]
	public string? Path { get; set; }
	[XmlAttribute(AttributeName = "offset")]
	public string? Offset { get; set; }
	[XmlAttribute(AttributeName = "offsetend")]
	public string? Offsetend { get; set; }
	[XmlAttribute(AttributeName = "sl")]
	public string? Sl { get; set; }
	[XmlAttribute(AttributeName = "fileid")]
	public string? Fileid { get; set; }
}

[XmlRoot(ElementName = "BranchPoints")]
public class BranchPoints
{
	[XmlElement(ElementName = "BranchPoint")]
	public List<BranchPoint>? BranchPoint { get; set; }
}

[XmlRoot(ElementName = "MethodPoint")]
public class MethodPoint
{
	[XmlAttribute(AttributeName = "vc")]
	public string? Vc { get; set; }
	[XmlAttribute(AttributeName = "uspid")]
	public string? Uspid { get; set; }
	[XmlAttribute(AttributeName = "type", Namespace = "xsi")]
	public string? Type { get; set; }
	[XmlAttribute(AttributeName = "ordinal")]
	public string? Ordinal { get; set; }
	[XmlAttribute(AttributeName = "offset")]
	public string? Offset { get; set; }
	[XmlAttribute(AttributeName = "sc")]
	public string? Sc { get; set; }
	[XmlAttribute(AttributeName = "sl")]
	public string? Sl { get; set; }
	[XmlAttribute(AttributeName = "ec")]
	public string? Ec { get; set; }
	[XmlAttribute(AttributeName = "el")]
	public string? El { get; set; }
	[XmlAttribute(AttributeName = "bec")]
	public string? Bec { get; set; }
	[XmlAttribute(AttributeName = "bev")]
	public string? Bev { get; set; }
	[XmlAttribute(AttributeName = "fileid")]
	public string? Fileid { get; set; }
	[XmlAttribute(AttributeName = "p8", Namespace = "http://www.w3.org/2000/xmlns/")]
	public string? P8 { get; set; }
}

[XmlRoot(ElementName = "Method")]
public class Method
{
	[XmlElement(ElementName = "Summary")]
	public Summary? Summary { get; set; }
	[XmlElement(ElementName = "MetadataToken")]
	public string? MetadataToken { get; set; }
	[XmlElement(ElementName = "Name")]
	public string Name { get; set; }
	[XmlElement(ElementName = "FileRef")]
	public FileRef? FileRef { get; set; }
	[XmlElement(ElementName = "SequencePoints")]
	public SequencePoints? SequencePoints { get; set; }
	[XmlElement(ElementName = "BranchPoints")]
	public BranchPoints? BranchPoints { get; set; }
	[XmlElement(ElementName = "MethodPoint")]
	public MethodPoint? MethodPoint { get; set; }
	[XmlAttribute(AttributeName = "cyclomaticComplexity")]
	public string? CyclomaticComplexity { get; set; }
	[XmlAttribute(AttributeName = "nPathComplexity")]
	public string? NPathComplexity { get; set; }
	[XmlAttribute(AttributeName = "sequenceCoverage")]
	public string SequenceCoverage { get; set; }
	[XmlAttribute(AttributeName = "branchCoverage")]
	public string BranchCoverage { get; set; }
	[XmlAttribute(AttributeName = "isConstructor")]
	public string? IsConstructor { get; set; }
	[XmlAttribute(AttributeName = "isGetter")]
	public string? IsGetter { get; set; }
	[XmlAttribute(AttributeName = "isSetter")]
	public string? IsSetter { get; set; }
	[XmlAttribute(AttributeName = "isStatic")]
	public string? IsStatic { get; set; }
}

[XmlRoot(ElementName = "Methods")]
public class Methods
{
	[XmlElement(ElementName = "Method")]
	public List<Method> Method { get; set; }
}

[XmlRoot(ElementName = "Class")]
public class Class
{
	[XmlElement(ElementName = "Summary")]
	public Summary Summary { get; set; }
	[XmlElement(ElementName = "FullName")]
	public string FullName { get; set; }
	[XmlElement(ElementName = "Methods")]
	public Methods Methods { get; set; }
}

[XmlRoot(ElementName = "Classes")]
public class Classes
{
	[XmlElement(ElementName = "Class")]
	public List<Class> Class { get; set; }
}

[XmlRoot(ElementName = "Module")]
public class Module
{
	[XmlElement(ElementName = "ModulePath")]
	public string? ModulePath { get; set; }
	[XmlElement(ElementName = "ModuleTime")]
	public string? ModuleTime { get; set; }
	[XmlElement(ElementName = "ModuleName")]
	public string ModuleName { get; set; }
	[XmlElement(ElementName = "Files")]
	public Files? Files { get; set; }
	[XmlElement(ElementName = "Classes")]
	public Classes Classes { get; set; }
	[XmlAttribute(AttributeName = "hash")]
	public string? Hash { get; set; }
}

[XmlRoot(ElementName = "Modules")]
public class Modules
{
	[XmlElement(ElementName = "Module")]
	public List<Module> Module { get; set; }
}

[XmlRoot(ElementName = "CoverageSession")]
public class CoverageSession
{
	[XmlElement(ElementName = "Summary")]
	public Summary Summary { get; set; }
	[XmlElement(ElementName = "Modules")]
	public Modules Modules { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
