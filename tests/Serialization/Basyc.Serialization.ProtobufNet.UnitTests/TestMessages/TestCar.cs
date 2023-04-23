namespace Basyc.Serialization.ProtobufNet.UnitTests.TestMessages;

public class TestCar
{
#pragma warning disable CS8618
    public TestCar()
#pragma warning restore CS8618
    {
    }

    public TestCar(string name, DateTime assemblyDate)
    {
        Name = name;
        AssemblyDate = assemblyDate;
    }

    public string Name { get; init; }
    public DateTime AssemblyDate { get; init; }
}
