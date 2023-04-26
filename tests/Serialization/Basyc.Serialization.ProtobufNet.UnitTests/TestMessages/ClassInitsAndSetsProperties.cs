namespace Basyc.Serialization.ProtobufNet.UnitTests.TestMessages;

#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only

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
