namespace Basyc.Serialization.ProtobufNet.UnitTests.TestMessages;

public class ClassInitsAndSetsProperties
{
	public ClassInitsAndSetsProperties(string name)
	{
		Name = name;
	}

	public string Name { get; init; }

	public string? NickName { get; set; }
	public List<string>? NickNames { get; set; }

	public int Age { get; init; } = 5;

	public List<int> Ages { get; init; } = new()
	{
		1,
		2,
		3
	};
}
